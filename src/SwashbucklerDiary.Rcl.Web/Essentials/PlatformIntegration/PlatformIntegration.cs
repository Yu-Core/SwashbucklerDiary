using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Web.Essentials
{
    public abstract partial class PlatformIntegration : IPlatformIntegration
    {
        private readonly ILogger _logger;

        private readonly PlatformIntegrationJSModule _jsModule;

        private readonly IAlertService _alertService;

        public PlatformIntegration(ILogger<PlatformIntegration> logger,
            PlatformIntegrationJSModule jsModule,
            IAlertService alertService)
        {
            _logger = logger;
            _jsModule = jsModule;
            _alertService = alertService;
        }
    }
}
