namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        public void OpenPlatformSetting() => AppInfo.Current.ShowSettingsUI();
    }
}
