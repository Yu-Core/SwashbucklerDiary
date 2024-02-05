using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration : IPlatformIntegration
    {
        private readonly ILogger _logger;

        private readonly Rcl.Essentials.IVersionTracking _versionTracking;

        public PlatformIntegration(ILogger<PlatformIntegration> logger,
            Rcl.Essentials.IVersionTracking versionTracking)
        {
            _logger = logger;
            _versionTracking = versionTracking;
        }
    }
}
