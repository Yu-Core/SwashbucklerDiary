using SwashbucklerDiary.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class DiaryCardSetting : PageComponentBase
    {
        private bool DiaryCardIcon;

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await base.OnInitializedAsync();
        }

        private async Task LoadSettings()
        {
            DiaryCardIcon = await SettingsService.Get(SettingType.DiaryCardIcon);
        }
    }
}
