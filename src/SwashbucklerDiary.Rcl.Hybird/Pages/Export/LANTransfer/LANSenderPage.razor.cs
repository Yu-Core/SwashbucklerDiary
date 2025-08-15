using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Services;
using System.Net.Sockets;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class LANSenderPage : ImportantComponentBase
    {
        private TransferDialog? transferDialog;

        private string? filePath;

        private readonly string multicastAddress = "239.0.0.1";

        private readonly int millisecondsOutTime = 20000;

        private int multicastPort;

        private int tcpPort;

        private bool showTransferDialog;

        private string? transferDialogTitle;

        private readonly List<LANDeviceInfoListItem> lanDeviceInfoListItems = [];

        [Inject]
        private ILANSenderService LANSenderService { get; set; } = default!;

        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;

        [Inject]
        private IDiaryFileManager DiaryFileManager { get; set; } = default!;

        [Inject]
        private IGlobalConfiguration GlobalConfiguration { get; set; } = default!;

        [Inject]
        private ILogger<LANSenderPage> Logger { get; set; } = default!;

        private class LANDeviceInfoListItem : LANDeviceInfo
        {
            public LANDeviceInfoListItem(LANDeviceInfo deviceInfo)
            {
                DeviceName = deviceInfo.DeviceName;
                IPAddress = deviceInfo.IPAddress;
                DevicePlatform = deviceInfo.DevicePlatform;
            }

            public string? DeviceIcon { get; set; }

            public Func<Task> OnClick { get; set; } = () => Task.CompletedTask;
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LANSenderService.DeviceDiscovered += HandleDeviceDiscovered;
            LANSenderService.DeviceTimeouted += HandleDeviceTimeouted;
            LANSenderService.SearchEnded += HandleSearchEnded;
            LANSenderService.SendCompleted += HandleSendingCompleted;
            LANSenderService.SendAborted += HandleSendAborted;
            LANSenderService.SendCanceled += HandleSendCanceled;
            LANSenderService.ConnectFailed += HandleConnectFailed;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await InitializeLANSenderService();
                StateHasChanged();
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            showTransferDialog = false;
            LANSenderService.Dispose();
            LANSenderService.DeviceDiscovered -= HandleDeviceDiscovered;
            LANSenderService.DeviceTimeouted -= HandleDeviceTimeouted;
            LANSenderService.SearchEnded -= HandleSearchEnded;
            LANSenderService.SendCompleted -= HandleSendingCompleted;
            LANSenderService.SendAborted -= HandleSendAborted;
            LANSenderService.SendCanceled -= HandleSendCanceled;
            LANSenderService.ConnectFailed -= HandleConnectFailed;
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            multicastPort = SettingService.Get(s => s.LANScanPort);
            tcpPort = SettingService.Get(s => s.LANTransmissionPort);
        }

        private async Task InitializeLANSenderService()
        {
            try
            {
                LANSenderService.Start(multicastAddress, multicastPort, millisecondsOutTime, tcpPort);
                return;
            }
            catch (SocketException e) when (e.SocketErrorCode == SocketError.NetworkUnreachable)
            {
                await AlertService.ErrorAsync(I18n.T("No network connection"));
            }
            catch (Exception e)
            {
                Logger.LogError(e, "LANSenderService initialize error");
                await AlertService.ErrorAsync(e.ToString());
            }

            await Task.Delay(1000);
            await NavigateToBack();
        }

        private async Task Send(string? ipAddress)
        {
            if (string.IsNullOrWhiteSpace(ipAddress))
            {
                return;
            }

            if (LANSenderService.IsSending)
            {
                return;
            }

            AlertService.StartLoading(I18n.T("Generating"));
            if (filePath == null)
            {
                var diaries = await DiaryService.QueryDiariesAsync();
                if (diaries.Count == 0)
                {
                    await AlertService.InfoAsync(I18n.T("No diary"));
                    AlertService.StopLoading();
                    return;
                }

                diaries = diaries.OrderBy(it => it.CreateTime).ToList();
                filePath = await DiaryFileManager.ExportJsonAsync(diaries);
            }

            AlertService.StopLoading();

            ResetProgress();
            transferDialogTitle = "Sending";
            showTransferDialog = true;
            StateHasChanged();

            await LANSenderService.SendAsync(ipAddress, filePath, new Progress<TransferProgressArguments>(HandleProgressChanged));
        }

        private void HandleDeviceDiscovered(LANDeviceInfo deviceInfo)
        {
            if (!lanDeviceInfoListItems.Any(it => it.IPAddress == deviceInfo.IPAddress))
            {
                LANDeviceInfoListItem lanDeviceInfoListItem = new(deviceInfo)
                {
                    DeviceIcon = GlobalConfiguration.GetPlatformIcon(deviceInfo.DevicePlatform),
                    OnClick = () => Send(deviceInfo.IPAddress)
                };

                lanDeviceInfoListItems.Add(lanDeviceInfoListItem);
                InvokeAsync(StateHasChanged);
            }
        }

        private void HandleProgressChanged(TransferProgressArguments args)
        {
            transferDialog?.SetProgress(args.TransferredBytes, args.TotalBytes);
        }

        private void HandleSendingCompleted()
        {
            InvokeAsync(async () =>
            {
                transferDialogTitle = "Send successfully";
                await AlertService.SuccessAsync(I18n.T("Send successfully"));
                StateHasChanged();
            });
        }

        private void HandleSendAborted()
        {
            InvokeAsync(async () =>
            {
                transferDialogTitle = "Send failed";
                await AlertService.ErrorAsync(I18n.T("Send failed"));
                StateHasChanged();
            });
        }

        private async Task CancelSend()
        {
            showTransferDialog = false;
            if (LANSenderService.IsSending)
            {
                LANSenderService.CancelSend();
            }

            await NavigateToBack();
        }

        private void HandleSearchEnded()
        {
            InvokeAsync(StateHasChanged);
        }

        private void ResetProgress()
        {
            transferDialog?.SetProgress(0, 0);
        }

        private void HandleSendCanceled()
        {
            InvokeAsync(async () =>
            {
                transferDialogTitle = "Send canceled";
                await AlertService.ErrorAsync(I18n.T("Send canceled"));
                StateHasChanged();
            });
        }

        private async void HandleConnectFailed(SocketException e)
        {
            if (e.SocketErrorCode == SocketError.ConnectionRefused)
            {
                await AlertService.ErrorAsync(I18n.T("Target does not exist"));
                return;
            }
            else if (e.SocketErrorCode == SocketError.NetworkUnreachable)
            {
                await AlertService.ErrorAsync(I18n.T("No network connection"));
            }
            else
            {
                await AlertService.ErrorAsync(I18n.T("Connection exception"));
            }

            await Task.Delay(1000);
            await NavigateToBack();
        }

        private void HandleDeviceTimeouted(LANDeviceInfo deviceInfo)
        {
            var item = lanDeviceInfoListItems.FirstOrDefault(it => it.IPAddress == deviceInfo.IPAddress);
            if (item != null)
            {
                lanDeviceInfoListItems.Remove(item);
                InvokeAsync(StateHasChanged);
            }
        }
    }
}
