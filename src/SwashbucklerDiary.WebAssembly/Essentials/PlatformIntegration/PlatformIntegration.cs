using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration : IPlatformIntegration
    {
        private readonly ILogger _logger;

        private readonly PlatformIntegrationJsModule _jsModule;

        public PlatformIntegration(ILogger<PlatformIntegration> logger,
            PlatformIntegrationJsModule jsModule)
        {
            _logger = logger;
            _jsModule = jsModule;
        }
    }
}
