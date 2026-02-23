namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration : Rcl.Web.Essentials.PlatformIntegration
    {
        public PlatformIntegration(ILogger<PlatformIntegration> logger,
            PlatformIntegrationJSModule jsModule) : base(logger, jsModule)
        {
        }
    }
}
