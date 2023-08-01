using MauiBlazorToolkit.Essentials;

namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
#if WINDOWS
        private readonly static string AppId = "9P6PBVBF466L";
#elif ANDROID
        private readonly static string AppId = AppInfo.PackageName;
#else
        private readonly static string AppId = string.Empty;
#endif
        //打开本应用的应用商店详情页
        public Task<bool> OpenStoreMyAppDetails()
        {
            return OpenStoreAppDetails(AppId);
        }

        private static Task<bool> OpenStoreAppDetails(string appId)
        {
#if WINDOWS
            return AppStoreLauncher.Default.TryOpenAsync(appId);
#elif ANDROID
            return OpenCoolmarket(appId);
#else
            return AppStoreLauncher.Default.TryOpenAsync();
#endif
        }
#if ANDROID
        private static Task<bool> OpenCoolmarket(string packageName)
        {
            string uri = $"coolmarket://apk/{packageName}";
            try
            {
                return Browser.Default.OpenAsync(uri);
            }
            catch (Exception)
            {
                uri = $"https://www.coolapk.com/apk/{packageName}";
                return Browser.Default.OpenAsync(uri);
            }
        }
#endif
    }
}
