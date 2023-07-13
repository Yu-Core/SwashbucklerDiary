namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        public string GetAppVersion()
        {
            return VersionTracking.Default.CurrentVersion.ToString();
        }

        public bool IsFirstLaunch()
        {
            return VersionTracking.Default.IsFirstLaunchEver;
        }
    }
}
