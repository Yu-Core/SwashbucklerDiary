using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class LANReceiverPage
    {
        private int ps;

        private long totalBytes;

        private long bytes;

        private readonly string multicastAddress = "239.0.0.1";

        private int multicastPort;

        private int tcpPort;

        private string? deviceName;

        private bool showTransferDialog;

        private string transferDialogTitle = "lanReceiver.Receiving";

        [Inject]
        private ILANReceiverService LANReceiverService { get; set; } = default!;

        [Inject]
        private IDiaryFileManager DiaryFileManager { get; set; } = default!;

        [Inject]
        private ILogger<LANReceiverPage> Logger { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LANReceiverService.ReceiveStart += ReceiveStart;
            LANReceiverService.ReceiveAborted += ReceiveAborted;
            LANReceiverService.ReceiveCompleted += ReceiveCompleted;
            LANReceiverService.ReceiveProgressChanged += ReceiveProgressChanged;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await InitializeLANReceiverService();
                StateHasChanged();
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            showTransferDialog = false;
            LANReceiverService.Dispose();
            LANReceiverService.ReceiveStart -= ReceiveStart;
            LANReceiverService.ReceiveAborted -= ReceiveAborted;
            LANReceiverService.ReceiveCompleted -= ReceiveCompleted;
            LANReceiverService.ReceiveProgressChanged -= ReceiveProgressChanged;
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            multicastPort = SettingService.Get(s => s.LANScanPort);
            tcpPort = SettingService.Get(s => s.LANTransmissionPort);
            deviceName = SettingService.Get(s => s.LANDeviceName);
        }

        private async Task InitializeLANReceiverService()
        {
            try
            {
                LANReceiverService.Initialize(multicastAddress, multicastPort, tcpPort, deviceName);
            }
            catch (Exception e)
            {
                Logger.LogError(e, "LANReceiverService initialize error");
                await PopupServiceHelper.Error(I18n.T("lanSender.No network connection"));
                await Task.Delay(1000);
                await NavigateToBack();
            }
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

        private void ReceiveStart()
        {
            LANReceiverService.CancelMulticast();
            ResetSendProgress();
            showTransferDialog = true;
            InvokeAsync(StateHasChanged);
        }

        private void ReceiveProgressChanged(long readLength, long allLength)
        {
            //只有每次传输大于512k且达到1%以上才会刷新进度
            var percentage = (int)((double)readLength / allLength * 100);
            bool refresh = (percentage - ps > 1 && readLength / 1024 - bytes > 512) || percentage == 100;

            if (refresh)
            {
                //在传输完成后，日记没有导入完之前，进度要保持在99%
                ps = percentage < 100 ? percentage : 99; //传输完成百分比
                bytes = readLength < allLength ? readLength / 1024 : (allLength / 1024) - 1; //当前已经传输的Kb
                totalBytes = allLength / 1024; //文件总大小Kb
                InvokeAsync(StateHasChanged);
            }
        }

        private void ReceiveAborted()
        {
            InvokeAsync(async () =>
            {
                if (showTransferDialog)
                {
                    transferDialogTitle = "lanReceiver.Receive failed";
                    await PopupServiceHelper.Error(I18n.T("lanReceiver.Receive failed"));
                }
                else
                {
                    transferDialogTitle = "lanReceiver.Receive canceled";
                    await PopupServiceHelper.Error(I18n.T("lanReceiver.Receive canceled"));
                }

                StateHasChanged();
            });
        }

        private void ReceiveCompleted(string path)
        {
            InvokeAsync(async () =>
            {
                bool isSuccess = await DiaryFileManager.ImportJsonAsync(path);

                if (!isSuccess)
                {
                    transferDialogTitle = "Export.Import.Fail";
                    await PopupServiceHelper.Error(I18n.T("Export.Import.Fail"));
                }
                else
                {
                    ps = 100;
                    bytes = totalBytes;
                    transferDialogTitle = "lanReceiver.Receive successfully";
                    await PopupServiceHelper.Success(I18n.T("lanReceiver.Receive successfully"));
                }

                StateHasChanged();
            });
        }

        private void ResetSendProgress()
        {
            ps = 0;
            bytes = 0;
            totalBytes = 0;
        }
    }
}
