using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration : IPlatformIntegration
    {
        private readonly ILogger _logger;

        private readonly II18nService _i18n;

        public PlatformIntegration(ILogger<PlatformIntegration> logger,
            II18nService i18n)
        {
            _logger = logger;
            _i18n = i18n;
        }
    }
}
