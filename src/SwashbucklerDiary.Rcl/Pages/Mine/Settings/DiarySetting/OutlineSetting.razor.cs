using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class OutlineSetting : ImportantComponentBase
    {
        private bool showOutline;

        private bool rightOutline;

        protected override void ReadSettings()
        {
            base.ReadSettings();

            showOutline = SettingService.Get(s => s.Outline);
            rightOutline = SettingService.Get(s => s.RigthOutline);
        }
    }
}
