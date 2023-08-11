using SwashbucklerDiary.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class AlertSettingPage : PageComponentBase
    {
        private int timeout;

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await base.OnInitializedAsync();
        }

        private async Task LoadSettings()
        {
            timeout = await SettingsService.Get(SettingType.AlertTimeout);
        }

        private async Task UpdateAlertTimeout(int value)
        {
            timeout = value;
            await SettingsService.Save(SettingType.AlertTimeout, value);
        }
    }
}
