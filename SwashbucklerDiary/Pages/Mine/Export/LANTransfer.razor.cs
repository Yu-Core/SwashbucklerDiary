using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Extend;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class LANTransfer : PageComponentBase
    {
        private bool ShowConfig;
        private List<DynamicListItem> DynamicLists = new();
        private LANConfigForm configModel = new();
        private string? DeviceName;

        [Inject]
        private ILANService LANService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            LoadView();
            await LoadSettings();
            await base.OnInitializedAsync();
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
            configModel.DeviceName = await SettingsService.Get(SettingType.LANDeviceName);
            DeviceName = LANService.GetLocalDeviceName();
            if (string.IsNullOrEmpty(configModel.DeviceName))
            {
                configModel.DeviceName = DeviceName;
            }
            configModel.ScanPort = await SettingsService.Get(SettingType.LANScanPort);
            configModel.TransmissionPort = await SettingsService.Get(SettingType.LANTransmissionPort);
        }

        private async Task SaveConfig(LANConfigForm value)
        {
            ShowConfig = false;
            configModel = value.DeepCopy();
            if(configModel.DeviceName != DeviceName)
            {
                await SettingsService.Save(SettingType.LANDeviceName, configModel.DeviceName);
            }
            
            await SettingsService.Save(SettingType.LANScanPort, configModel.ScanPort);
            await SettingsService.Save(SettingType.LANTransmissionPort, configModel.TransmissionPort);
        }

        private async Task Reset()
        {
            ShowConfig = false;
            await SettingsService.Remove(SettingType.LANDeviceName);
            await SettingsService.Remove(SettingType.LANScanPort);
            await SettingsService.Remove(SettingType.LANTransmissionPort);
            await LoadSettings();
        } 
    }
}
