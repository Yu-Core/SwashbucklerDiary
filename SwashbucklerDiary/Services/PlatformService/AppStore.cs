using MauiBlazorToolkit.Essentials;

namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        private readonly static string MicrosoftStoreId = "9P6PBVBF466L";
        //打开本应用的应用商店详情页
        public Task<bool> OpenStoreMyAppDetails()
        {
#if WINDOWS
            return OpenStoreAppDetails(MicrosoftStoreId);
#elif ANDROID
            return OpenCoolmarket(AppInfo.PackageName);
#else
            return OpenStoreAppDetails(AppInfo.PackageName);
#endif
        }

        private static Task<bool> OpenStoreAppDetails(string appId)
        {
            return AppStoreLauncher.Default.TryOpenAsync(appId);
        }

        private Task<bool> OpenCoolmarket(string packageName)
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
    }
}
