namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        public string AppVersionString
            => AppInfo.VersionString;

        public Task ShowSettingsUI()
        {
            AppInfo.Current.ShowSettingsUI();
            return Task.CompletedTask;
        }
    }
}
