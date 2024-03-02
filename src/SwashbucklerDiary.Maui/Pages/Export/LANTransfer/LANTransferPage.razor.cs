using SwashbucklerDiary.Maui.Services;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Pages
{
    public partial class LANTransferPage : ImportantComponentBase
    {
        private bool ShowConfig;

        private List<DynamicListItem> DynamicLists = new();

        private LANConfigForm configModel = new();

        private string? defaultDeviceName;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LoadView();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await LoadSettings();
                StateHasChanged();
            }
        }

        private void LoadView()
        {
            DynamicLists = new()
            {
                new(this,"Export.Send.Name","mdi-send-outline",()=>To("lanSender")),
                new(this,"Export.Receive.Name","mdi-printer-pos-outline",()=>To("lanReceiver")),
            };
        }

        private async Task LoadSettings()
        {
            configModel.DeviceName = await SettingService.Get<string>(Setting.LANDeviceName);
            defaultDeviceName = LANHelper.GetLocalDeviceName();
            if (string.IsNullOrEmpty(configModel.DeviceName))
            {
                configModel.DeviceName = defaultDeviceName;
            }

            configModel.ScanPort = await SettingService.Get<int>(Setting.LANScanPort);
            configModel.TransmissionPort = await SettingService.Get<int>(Setting.LANTransmissionPort);
        }

        private async Task SaveConfig(LANConfigForm value)
        {
            ShowConfig = false;
            configModel = value.DeepCopy();
            if (configModel.DeviceName != defaultDeviceName)
            {
                await SettingService.Set(Setting.LANDeviceName, configModel.DeviceName);
            }

            await SettingService.Set(Setting.LANScanPort, configModel.ScanPort);
            await SettingService.Set(Setting.LANTransmissionPort, configModel.TransmissionPort);
        }

        private async Task Reset()
        {
            ShowConfig = false;
            await SettingService.Remove(Setting.LANDeviceName);
            await SettingService.Remove(Setting.LANScanPort);
            await SettingService.Remove(Setting.LANTransmissionPort);
            await LoadSettings();
        }
    }
}
