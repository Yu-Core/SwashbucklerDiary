using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class IndexSetting : ImportantComponentBase
    {
        private bool welcomText;

        private bool date;

        protected override void ReadSettings()
        {
            welcomText = SettingService.Get<bool>(Setting.WelcomeText);
            date = SettingService.Get<bool>(Setting.IndexDate);
        }
    }
}
