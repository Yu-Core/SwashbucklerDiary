using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class IndexSetting : ImportantComponentBase
    {
        private bool welcomText;

        private bool date;

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
            welcomText = await SettingService.Get<bool>(Setting.WelcomeText);
            date = await SettingService.Get<bool>(Setting.IndexDate);
        }
    }
}
