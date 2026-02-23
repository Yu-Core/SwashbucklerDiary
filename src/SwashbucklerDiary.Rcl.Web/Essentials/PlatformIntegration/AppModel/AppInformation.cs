namespace SwashbucklerDiary.Rcl.Web.Essentials
{
    public abstract partial class PlatformIntegration
    {
        public abstract string AppVersionString { get; }

        public Task ShowSettingsUI()
        {
            //TODO: Browser environment cannot open browser settings, But it can be done through browser plugins or tampermonkey scripts
            //return OpenUri("edge://settings/profiles", true);

            return Task.CompletedTask;
        }
    }
}
