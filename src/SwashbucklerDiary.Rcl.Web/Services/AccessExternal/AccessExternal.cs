using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;

namespace SwashbucklerDiary.Rcl.Web.Services
{
    public class AccessExternal : Rcl.Services.AccessExternal
    {
        private readonly Lazy<ValueTask<IJSObjectReference>> _module;

        private readonly Lazy<Task<string>> _joinQQGroupUrl;

        private readonly Lazy<Task<Dictionary<string, string>>> _appIds;

        private Task<string> JoinQQGroupUrl => _joinQQGroupUrl.Value;

        private Task<Dictionary<string, string>> AppIds => _appIds.Value;

        private ValueTask<IJSObjectReference> Module => _module.Value;

        public AccessExternal(IPlatformIntegration platformIntegration,
            IStaticWebAssets staticWebAssets,
            IJSRuntime jS
            ) : base(staticWebAssets, platformIntegration)
        {
            _joinQQGroupUrl = new(() => _staticWebAssets.ReadJsonAsync<string>("json/qq-group/qq-group.json"));
            _appIds = new(() => _staticWebAssets.ReadJsonAsync<Dictionary<string, string>>("json/app-id/app-id.json"));
            _module = new(() => jS.ImportJsModule("/_content/SwashbucklerDiary.Rcl.Web/js/accessExternal.js"));
        }

        public override async Task<bool> OpenAppStoreAppDetails()
        {
            var appIds = await AppIds.ConfigureAwait(false);
            var module = await Module.ConfigureAwait(false);
            return await module.InvokeAsync<bool>("openAppStoreAppDetails", appIds).ConfigureAwait(false);
        }

        public override async Task<bool> JoinQQGroup()
        {
            var joinQQGroupUrl = await JoinQQGroupUrl.ConfigureAwait(false);
            return await _platformIntegration.OpenBrowser(joinQQGroupUrl).ConfigureAwait(false);
        }
    }
}
