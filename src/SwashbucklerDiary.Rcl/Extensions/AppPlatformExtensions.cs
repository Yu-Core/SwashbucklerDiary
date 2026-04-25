using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static class AppPlatformExtensions
    {
        public static bool IsInAppStore(this AppPlatform platform)
        {
            return platform is WinUIPackagedAppPlatform
                || (platform.OS == AppOperatingSystem.Android);
        }
    }
}
