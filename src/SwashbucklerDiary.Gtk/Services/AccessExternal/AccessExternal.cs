using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Gtk.Services
{
    public class AccessExternal : Rcl.Services.AccessExternal
    {
        private readonly Lazy<Task<string>> _joinQQGroupUrl;

        private Task<string> JoinQQGroupUrl => _joinQQGroupUrl.Value;

        public AccessExternal(IPlatformIntegration platformIntegration,
            IStaticWebAssets staticWebAssets
            ) : base(staticWebAssets, platformIntegration)
        {
            _joinQQGroupUrl = new(() => _staticWebAssets.ReadJsonAsync<string>("json/qq-group/qq-group.json"));
        }

        public override Task<bool> OpenAppStoreAppDetails()
        {
            return Task.FromResult(false);
        }

        public override async Task<bool> JoinQQGroup()
        {
            var joinQQGroupUrl = await JoinQQGroupUrl;
            return await _platformIntegration.OpenBrowser(joinQQGroupUrl);
        }
    }
}
