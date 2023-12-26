using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class IndexSetting : ImportantComponentBase
    {
        private bool welcomText;

        private bool date;

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await base.OnInitializedAsync();
        }

        private async Task LoadSettings()
        {
            welcomText = await Preferences.Get<bool>(Setting.WelcomeText);
            date = await Preferences.Get<bool>(Setting.Date);
        }
    }
}
