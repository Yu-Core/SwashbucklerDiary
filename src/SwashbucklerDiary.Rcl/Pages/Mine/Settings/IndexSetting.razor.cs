using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class IndexSetting : ImportantComponentBase
    {
        private bool welcomText;

        private bool date;

        protected override void ReadSettings()
        {
            welcomText = SettingService.Get(s => s.WelcomeText);
            date = SettingService.Get(s => s.IndexDate);
        }
    }
}
