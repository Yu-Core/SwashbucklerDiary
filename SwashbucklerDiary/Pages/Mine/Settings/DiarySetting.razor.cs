using SwashbucklerDiary.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class DiarySetting : PageComponentBase
    {
        private bool Title;
        private bool Markdown;
        private bool EditCreateTime;

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await base.OnInitializedAsync();
        }

        private async Task LoadSettings()
        {
            Title = await SettingsService.Get(SettingType.Title);
            Markdown = await SettingsService.Get(SettingType.Markdown);
            EditCreateTime = await SettingsService.Get(SettingType.EditCreateTime);
        }
    }
}
