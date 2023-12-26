using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class DiarySetting : ImportantComponentBase
    {
        private bool title;

        private bool markdown;

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await base.OnInitializedAsync();
        }

        private async Task LoadSettings()
        {
            title = await Preferences.Get<bool>(Setting.Title);
            markdown = await Preferences.Get<bool>(Setting.Markdown);
        }
    }
}
