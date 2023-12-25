using MauiBlazorToolkit.Essentials;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Maui.Services
{
    public class AccessExternal : Rcl.Services.AccessExternal
    {
        private readonly Lazy<string> appId;

        private string AppId => appId.Value;

        public AccessExternal(IPlatformIntegration platformIntegration,
            IStaticWebAssets staticWebAssets
            ) : base(staticWebAssets, platformIntegration)
        {
            //默认加入QQ群的Url是从网页跳转QQ，代码位于Rcl
            //ANDROID || IOS || MACCATALYST 使用直接跳转的链接
#if ANDROID || IOS || MACCATALYST
#if ANDROID
            string joinQQGroupUrlJsonPath = "json/qq-group/qq-group.Android.json";
#elif IOS || MACCATALYST
            string joinQQGroupUrlJsonPath = "json/qq-group/qq-group.MaciOS.json";
#endif
            joinQQGroupUrl = new(() => _staticWebAssets.ReadJsonAsync<string>(joinQQGroupUrlJsonPath, false).Result);
#endif
            //读取应用商店的AppId
            //目前只有Windows和Android
#if WINDOWS || ANDROID
#if WINDOWS
            string appIdJsonPath = "json/app-id/app-id.Windows.json";
#elif ANDROID
            string appIdJsonPath = "json/app-id/app-id.Android.json";
#endif
            appId = new(() => _staticWebAssets.ReadJsonAsync<string>(appIdJsonPath, false).Result);
#else
            appId = new(() => string.Empty);
#endif
        }

#if ANDROID || IOS || MACCATALYST
        public override Task<bool> JoinQQGroup()
            => Launcher.Default.TryOpenAsync(JoinQQGroupUrl);
#endif

        public override Task<bool> OpenAppStoreAppDetails()
            => AppStoreLauncher.Default.TryOpenAsync(AppId);
    }
}
