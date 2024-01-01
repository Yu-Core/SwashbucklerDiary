using MauiBlazorToolkit.Essentials;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Services
{
    public class AccessExternal : Rcl.Services.AccessExternal
    {
        private readonly Lazy<string> _appId;

        private readonly Lazy<string> _joinQQGroupUrl;

        private string AppId => _appId.Value;

        private string JoinQQGroupUrl => _joinQQGroupUrl.Value;

        public AccessExternal(IPlatformIntegration platformIntegration,
            IStaticWebAssets staticWebAssets
            ) : base(staticWebAssets, platformIntegration)
        {
            //默认加入QQ群的Url是从网页跳转QQ，代码位于Rcl
            //ANDROID || IOS 使用直接跳转的链接
#if ANDROID || IOS
            _joinQQGroupUrl = new(() =>
            {
                var qqGroupUrls = _staticWebAssets.ReadJsonAsync<Dictionary<AppDevicePlatform, string>>("json/qq-group/qq-group.json", false).Result;
                return qqGroupUrls[_platformIntegration.CurrentPlatform];
            });
#else
            _joinQQGroupUrl = new(() => _staticWebAssets.ReadJsonAsync<string>("json/qq-group/qq-group.json").Result);
#endif
            //读取应用商店的AppId
            //目前只有Windows和Android
#if WINDOWS || ANDROID
            _appId = new(() =>
            {
                var appIds = _staticWebAssets.ReadJsonAsync<Dictionary<AppDevicePlatform, string>>("json/app-id/app-id.json").Result;
                return appIds[_platformIntegration.CurrentPlatform];
            });
#else
            _appId = new(() => string.Empty);
#endif
        }

        public override Task<bool> JoinQQGroup()
#if ANDROID || IOS
            => Launcher.Default.TryOpenAsync(JoinQQGroupUrl);
#else
            => _platformIntegration.OpenBrowser(JoinQQGroupUrl);
#endif

        public override Task<bool> OpenAppStoreAppDetails()
            => AppStoreLauncher.Default.TryOpenAsync(AppId);


    }
}
