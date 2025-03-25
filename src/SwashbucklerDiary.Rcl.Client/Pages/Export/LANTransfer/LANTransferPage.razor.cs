using DeepCloner.Core;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Models;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class LANTransferPage : ImportantComponentBase
    {
        private bool ShowConfig;

        private List<DynamicListItem> DynamicLists = [];

        private LANConfigForm configModel = new();

        private string? defaultDeviceName;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LoadView();
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            configModel.DeviceName = SettingService.Get(s => s.LANDeviceName);
            defaultDeviceName = PlatformIntegration.DeviceName;
            if (string.IsNullOrEmpty(configModel.DeviceName))
            {
                configModel.DeviceName = defaultDeviceName;
            }

            configModel.ScanPort = SettingService.Get(s => s.LANScanPort);
            configModel.TransmissionPort = SettingService.Get(s => s.LANTransmissionPort);
        }

        private void LoadView()
        {
            DynamicLists =
            [
                new(this,"Send","mdi-send-outline",()=>To("lanSender")),
                new(this,"Receive","mdi-printer-pos-outline",()=>To("lanReceiver")),
            ];
        }

        private async Task SaveConfig(LANConfigForm value)
        {
            ShowConfig = false;
            configModel = value.DeepClone();
            if (configModel.DeviceName != defaultDeviceName)
            {
                await SettingService.SetAsync(s => s.LANDeviceName, configModel.DeviceName);
            }

            await SettingService.SetAsync(s => s.LANScanPort, configModel.ScanPort);
            await SettingService.SetAsync(s => s.LANTransmissionPort, configModel.TransmissionPort);
        }

        private async Task Reset()
        {
            ShowConfig = false;
            await SettingService.RemoveAsync(s => s.LANDeviceName);
            await SettingService.RemoveAsync(s => s.LANScanPort);
            await SettingService.RemoveAsync(s => s.LANTransmissionPort);
            ReadSettings();
        }
    }
}
