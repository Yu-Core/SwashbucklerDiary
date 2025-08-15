using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.WebAssembly.Extensions;

namespace SwashbucklerDiary.WebAssembly.Services
{
    public class AccessExternal : Rcl.Services.AccessExternal
    {
        private readonly Lazy<ValueTask<IJSInProcessObjectReference>> _module;

        private readonly Lazy<Task<string>> _joinQQGroupUrl;

        private readonly Lazy<Task<Dictionary<string, string>>> _appIds;

        private Task<string> JoinQQGroupUrl => _joinQQGroupUrl.Value;

        private Task<Dictionary<string, string>> AppIds => _appIds.Value;

        private ValueTask<IJSInProcessObjectReference> Module => _module.Value;

        public AccessExternal(IPlatformIntegration platformIntegration,
            IStaticWebAssets staticWebAssets,
            IJSRuntime jS
            ) : base(staticWebAssets, platformIntegration)
        {
            _joinQQGroupUrl = new(() => _staticWebAssets.ReadJsonAsync<string>("json/qq-group/qq-group.json"));
            _appIds = new(() => _staticWebAssets.ReadJsonAsync<Dictionary<string, string>>("json/app-id/app-id.json"));
            _module = new(() => ((IJSInProcessRuntime)jS).ImportJsModule("js/accessExternal.js"));
        }

        public override async Task<bool> OpenAppStoreAppDetails()
        {
            var appIds = await AppIds.ConfigureAwait(false);
            var module = await Module.ConfigureAwait(false);
            return module.Invoke<bool>("openAppStoreAppDetails", appIds);
        }

        public override async Task<bool> JoinQQGroup()
        {
            var joinQQGroupUrl = await JoinQQGroupUrl.ConfigureAwait(false);
            return await _platformIntegration.OpenBrowser(joinQQGroupUrl).ConfigureAwait(false);
        }
    }
}
