using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration : IPlatformIntegration
    {
        private readonly ILogger _logger;

        public PlatformIntegration(ILogger<PlatformIntegration> logger)
        {
            _logger = logger;
        }
    }
}
