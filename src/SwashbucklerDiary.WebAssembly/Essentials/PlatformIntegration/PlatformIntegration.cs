using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration : IPlatformIntegration
    {
        private readonly ILogger _logger;

        private readonly PlatformIntegrationJSModule _jsModule;

        public PlatformIntegration(ILogger<PlatformIntegration> logger,
            PlatformIntegrationJSModule jsModule)
        {
            _logger = logger;
            _jsModule = jsModule;
        }
    }
}
