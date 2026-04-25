using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        public AppPlatform CurrentPlatform { get; }
#if WINDOWS
           = DetectWindowsPackaged() ? new WinUIPackagedAppPlatform() : new WinUIUnpackagedAppPlatform();
#elif ANDROID
           = new AndroidAppPlatform();
#elif MACCATALYST
           = new MacOSAppPlatform();
#elif IOS
           = new IOSAppPlatform();
#else
           = new UnknownAppPlatform();
#endif
        public string DeviceName => DeviceInfo.Current.Name;

#if WINDOWS
        private static bool DetectWindowsPackaged()
        {
            try
            {
                // 在 UWP 或打包的 .NET 应用中可调用
                var package = Windows.ApplicationModel.Package.Current;
                return package != null;
            }
            catch
            {
                return false; // 非打包环境
            }
        }
#endif
    }
}
