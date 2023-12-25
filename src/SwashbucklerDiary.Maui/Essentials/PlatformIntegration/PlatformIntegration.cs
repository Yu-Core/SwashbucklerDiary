using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration : IPlatformIntegration
    {
        private readonly IAlertService _alertService;

        private readonly II18nService _i18n;

        private readonly ILogger _logger;

        public PlatformIntegration(IAlertService alertService, 
            II18nService i18n,
            ILogger<PlatformIntegration> logger)
        {
            _alertService = alertService;
            _i18n = i18n;
            _logger = logger;
        }
    }
}
