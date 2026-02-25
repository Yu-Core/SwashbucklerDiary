using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Server.Essentials
{
    public partial class PlatformIntegration : Rcl.Web.Essentials.PlatformIntegration
    {
        public PlatformIntegration(ILogger<PlatformIntegration> logger,
            PlatformIntegrationJSModule jsModule,
            IAlertService alertService) : base(logger, jsModule, alertService)
        {
        }
    }
}
