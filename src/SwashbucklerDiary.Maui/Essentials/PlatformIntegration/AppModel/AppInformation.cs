namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        public Task<string> GetAppVersion()
        {
            var version = VersionTracking.Default.CurrentVersion.ToString();
            return Task.FromResult(version);
        }

        public Task ShowSettingsUI()
        {
            AppInfo.Current.ShowSettingsUI();
            return Task.CompletedTask;
        }
    }
}
