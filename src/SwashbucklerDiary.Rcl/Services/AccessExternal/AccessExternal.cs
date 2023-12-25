using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Services
{
    public abstract class AccessExternal : IAccessExternal
    {
        protected readonly IStaticWebAssets _staticWebAssets;

        protected readonly IPlatformIntegration _platformIntegration;

        protected Lazy<string> joinQQGroupUrl;

        protected string JoinQQGroupUrl => joinQQGroupUrl.Value;

        public AccessExternal(IStaticWebAssets staticWebAssets,
            IPlatformIntegration platformIntegration)
        {
            _staticWebAssets = staticWebAssets;
            _platformIntegration = platformIntegration;
            joinQQGroupUrl = new(() => _staticWebAssets.ReadJsonAsync<string>("json/qq-group/qq-group.json").Result);
        }

        public virtual Task<bool> JoinQQGroup()
            => _platformIntegration.OpenBrowser(JoinQQGroupUrl);

        public abstract Task<bool> OpenAppStoreAppDetails();
    }
}
