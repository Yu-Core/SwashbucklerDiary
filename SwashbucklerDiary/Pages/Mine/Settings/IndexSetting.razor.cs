using SwashbucklerDiary.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class IndexSetting : PageComponentBase
    {
        private bool WelcomText;
        private bool Date;

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await base.OnInitializedAsync();
        }

        private async Task LoadSettings()
        {
            WelcomText = await SettingsService.Get(SettingType.WelcomeText);
            Date = await SettingsService.Get(SettingType.Date);
        }
    }
}
