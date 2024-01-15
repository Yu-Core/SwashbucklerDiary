namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration
    {
        public string AppVersion
            => _versionTracking.CurrentVersion.ToString();

        public Task ShowSettingsUI()
        {
            //TODO: Browser environment cannot open browser settings, But it can be done through browser plugins or tampermonkey scripts
            //return OpenUri("edge://settings/profiles", true);

            return Task.CompletedTask;
        }
    }
}
