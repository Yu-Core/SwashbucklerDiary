using SwashbucklerDiary.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class DiarySetting : ImportantComponentBase
    {
        private bool Title;

        private bool Markdown;

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await base.OnInitializedAsync();
        }

        private async Task LoadSettings()
        {
            Title = await SettingsService.Get(SettingType.Title);
            Markdown = await SettingsService.Get(SettingType.Markdown);
        }
    }
}
