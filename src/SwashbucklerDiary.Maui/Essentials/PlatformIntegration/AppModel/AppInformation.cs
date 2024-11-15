namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        public string AppVersion
            => _versionTracking.CurrentVersion.ToString();

        public Task ShowSettingsUI()
        {
            AppInfo.Current.ShowSettingsUI();
            return Task.CompletedTask;
        }
    }
}
