using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class LANSenderPage : ImportantComponentBase
    {
        private int ps;

        private long totalBytes;

        private long bytes;

        private string? filePath;

        private readonly string multicastAddress = "239.0.0.1";

        private readonly int millisecondsOutTime = 20000;

        private int multicastPort;

        private int tcpPort;

        private bool showTransferDialog;

        private string transferDialogTitle = "lanSender.Sending";

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

            LANSenderService.LANDeviceFound += LANDeviceFound;
            LANSenderService.SearchEnded += SearchEnded;
            LANSenderService.SendProgressChanged += SendProgressChanged;
            LANSenderService.SendCompleted += SendingCompleted;
            LANSenderService.SendAborted += SendAborted;
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
            LANSenderService.LANDeviceFound -= LANDeviceFound;
            LANSenderService.SearchEnded -= SearchEnded;
            LANSenderService.SendProgressChanged -= SendProgressChanged;
            LANSenderService.SendCompleted -= SendingCompleted;
            LANSenderService.SendAborted -= SendAborted;
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
                LANSenderService.Initialize(multicastAddress, multicastPort, tcpPort, millisecondsOutTime);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "LANSenderService initialize error");
                await PopupServiceHelper.Error(I18n.T("lanSender.No network connection"));
                await Task.Delay(1000);
                await NavigateToBack();
            }
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

            ResetSendProgress();
            showTransferDialog = true;
            StateHasChanged();

            if (filePath == null)
            {
                var diaries = await DiaryService.QueryAsync();
                if (diaries.Count == 0)
                {
                    await PopupServiceHelper.Info(I18n.T("Diary.NoDiary"));
                    return;
                }

                diaries = diaries.OrderBy(it => it.CreateTime).ToList();
                filePath = await DiaryFileManager.ExportJsonAsync(diaries);
            }

            LANSenderService.Send(ipAddress, filePath);
        }

        private void LANDeviceFound(LANDeviceInfo deviceInfo)
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

        private void SendProgressChanged(long readLength, long allLength)
        {
            //只有每次传输大于512k且达到1%以上才会刷新进度
            var percentage = (int)(((double)readLength / allLength) * 100);
            bool refresh = (percentage - ps > 1 && readLength / 1024 - bytes > 512) || percentage == 100;

            if (refresh)
            {
                ps = percentage; //传输完成百分比
                bytes = readLength / 1024; //当前已经传输的Kb
                totalBytes = allLength / 1024; //文件总大小Kb
                InvokeAsync(StateHasChanged);
            }
        }

        private void SendingCompleted()
        {
            InvokeAsync(async () =>
            {
                transferDialogTitle = "lanSender.Send successfully";
                await PopupServiceHelper.Success(I18n.T("lanSender.Send successfully"));
                StateHasChanged();
            });
        }

        private void SendAborted()
        {
            InvokeAsync(async () =>
            {
                if (showTransferDialog)
                {
                    transferDialogTitle = "lanSender.Send failed";
                    await PopupServiceHelper.Error(I18n.T("lanSender.Send failed"));
                }
                else
                {
                    transferDialogTitle = "lanSender.Send canceled";
                    await PopupServiceHelper.Error(I18n.T("lanSender.Send canceled"));
                }

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

        private void SearchEnded()
        {
            InvokeAsync(StateHasChanged);
        }

        private void ResetSendProgress()
        {
            ps = 0;
            bytes = 0;
            totalBytes = 0;
        }
    }
}
