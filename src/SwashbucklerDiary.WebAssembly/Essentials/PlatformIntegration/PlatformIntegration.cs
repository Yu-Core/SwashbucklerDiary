using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.WebAssembly.Extensions;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public partial class PlatformIntegration : IPlatformIntegration
    {
        private readonly ILogger _logger;

        private readonly IVersionTracking _versionTracking;

        private readonly Lazy<ValueTask<IJSInProcessObjectReference>> _module;

        private ValueTask<IJSInProcessObjectReference> Module => _module.Value;

        public PlatformIntegration(ILogger<PlatformIntegration> logger,
            IJSRuntime jS,
            IVersionTracking versionTracking)
        {
            _logger = logger;
            _versionTracking = versionTracking;
            // import need async
            //https://github.com/dotnet/aspnetcore/issues/29808
            _module = new(() => ((IJSInProcessRuntime)jS).ImportJsModule("js/platformIntegration.js"));
        }
    }
}
