using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.WebAssembly.Services
{
    public class AccessExternal : Rcl.Services.AccessExternal
    {
        private readonly Lazy<Task<IJSInProcessObjectReference>> _module;

        private readonly Lazy<Task<string>> _joinQQGroupUrl;

        private readonly Lazy<Task<Dictionary<string, string>>> _appIds;

        private Task<string> JoinQQGroupUrl => _joinQQGroupUrl.Value;

        private Task<Dictionary<string, string>> AppIds => _appIds.Value;

        private Task<IJSInProcessObjectReference> Module => _module.Value;

        public AccessExternal(IPlatformIntegration platformIntegration,
            IStaticWebAssets staticWebAssets,
            IJSRuntime jS
            ) : base(staticWebAssets, platformIntegration)
        {
            _joinQQGroupUrl = new(() => _staticWebAssets.ReadJsonAsync<string>("json/qq-group/qq-group.json"));
            _appIds = new(() => _staticWebAssets.ReadJsonAsync<Dictionary<string, string>>("json/app-id/app-id.json"));
            _module = new(() => ((IJSInProcessRuntime)jS).InvokeAsync<IJSInProcessObjectReference>("import", "./js/accessExternal.js").AsTask());
        }

        public override async Task<bool> OpenAppStoreAppDetails()
        {
            var appIds = await AppIds;
            var module = await Module;
            return module.Invoke<bool>("openAppStoreAppDetails", appIds);
        }

        public override async Task<bool> JoinQQGroup()
        {
            var joinQQGroupUrl = await JoinQQGroupUrl;
            return await _platformIntegration.OpenBrowser(joinQQGroupUrl);
        }
    }
}
