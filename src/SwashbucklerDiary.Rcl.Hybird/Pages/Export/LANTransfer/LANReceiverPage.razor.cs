using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Services;
using System.Net.Sockets;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class LANReceiverPage
    {
        private TransferDialog? transferDialog;

        private double maxPercentage = 99;

        private readonly string multicastAddress = "239.0.0.1";

        private int multicastPort;

        private int tcpPort;

        private string? deviceName;

        private bool showTransferDialog;

        private string transferDialogTitle = "Receiving";

        [Inject]
        private ILANReceiverService LANReceiverService { get; set; } = default!;

        [Inject]
        private IDiaryFileManager DiaryFileManager { get; set; } = default!;

        [Inject]
        private ILogger<LANReceiverPage> Logger { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LANReceiverService.ReceiveStart += HandleReceiveStarted;
            LANReceiverService.ReceiveAborted += HandleReceiveAborted;
            LANReceiverService.ReceiveCanceled += HandleReceiveCanceled;
            LANReceiverService.ReceiveCompleted += HandleReceiveCompleted;
            LANReceiverService.ConnectFailed += HandleConnectFailed;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await InitializeLANReceiverServiceAsync();
                StateHasChanged();
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            showTransferDialog = false;
            LANReceiverService.Dispose();
            LANReceiverService.ReceiveStart -= HandleReceiveStarted;
            LANReceiverService.ReceiveAborted -= HandleReceiveAborted;
            LANReceiverService.ReceiveCanceled -= HandleReceiveCanceled;
            LANReceiverService.ReceiveCompleted -= HandleReceiveCompleted;
            LANReceiverService.ConnectFailed -= HandleConnectFailed;
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            multicastPort = SettingService.Get(s => s.LANScanPort);
            tcpPort = SettingService.Get(s => s.LANTransmissionPort);
            deviceName = SettingService.Get(s => s.LANDeviceName);
        }

        private async Task InitializeLANReceiverServiceAsync()
        {
            try
            {
                LANReceiverService.Start(multicastAddress, multicastPort, deviceName, tcpPort, new Progress<TransferProgressArguments>(HandleProgressChanged));
                return;
            }
            catch (SocketException e) when (e.SocketErrorCode == SocketError.NetworkUnreachable)
            {
                await AlertService.ErrorAsync(I18n.T("No network connection"));
            }
            catch (Exception e)
            {
                Logger.LogError(e, "LANReceiverService initialize error");
                await AlertService.ErrorAsync(e.ToString());
            }

            await Task.Delay(1000);
            await NavigateToBack();
        }

        private async Task CancelReceive()
        {
            showTransferDialog = false;
            if (LANReceiverService.IsReceiving)
            {
                LANReceiverService.CancelReceive();
            }

            await NavigateToBack();
        }

        private void HandleReceiveStarted()
        {
            LANReceiverService.CancelMulticast();
            ResetProgress();
            showTransferDialog = true;
            InvokeAsync(StateHasChanged);
        }

        private void HandleProgressChanged(TransferProgressArguments args)
        {
            transferDialog?.SetProgress(args.TransferredBytes, args.TotalBytes);
        }

        private void HandleReceiveAborted()
        {
            InvokeAsync(async () =>
            {
                transferDialogTitle = "Receive failed";
                await AlertService.ErrorAsync(I18n.T("Receive failed"));

                StateHasChanged();
            });
        }

        private void HandleReceiveCompleted(string path)
        {
            InvokeAsync(async () =>
            {
                AlertService.StartLoading(I18n.T("Importing"));

                bool isSuccess = await DiaryFileManager.ImportJsonAsync(path);

                if (!isSuccess)
                {
                    transferDialogTitle = "Import failed";
                    await AlertService.ErrorAsync(I18n.T("Import failed"));
                }
                else
                {
                    maxPercentage = 100;
                    transferDialogTitle = "Receive successfully";
                    await AlertService.SuccessAsync(I18n.T("Receive successfully"));
                }

                AlertService.StopLoading();

                StateHasChanged();
            });
        }

        private void ResetProgress()
        {
            maxPercentage = 99;
            transferDialog?.SetProgress(0, 0);
        }

        private void HandleReceiveCanceled()
        {
            InvokeAsync(async () =>
            {
                transferDialogTitle = "Receive canceled";
                await AlertService.ErrorAsync(I18n.T("Receive canceled"));

                StateHasChanged();
            });
        }

        private async void HandleConnectFailed(SocketException e)
        {
            if (e.SocketErrorCode == SocketError.NetworkUnreachable)
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
    }
}
