using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class DiarySetting : ImportantComponentBase
    {
        private bool title;

        private bool markdown;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await UpdateSettings();
                StateHasChanged();
            }
        }

        private async Task UpdateSettings()
        {
            title = await Preferences.Get<bool>(Setting.Title);
            markdown = await Preferences.Get<bool>(Setting.Markdown);
        }
    }
}
