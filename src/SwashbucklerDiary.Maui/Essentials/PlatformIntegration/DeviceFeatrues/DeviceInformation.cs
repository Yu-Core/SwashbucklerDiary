using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        public AppPlatform CurrentPlatform
#if WINDOWS
           => AppPlatform.Windows;
#elif ANDROID
           => AppPlatform.Android;
#elif MACCATALYST
           => AppPlatform.macOS;
#elif IOS
           => AppPlatform.iOS;
#elif TIZEN
           => AppDevicePlatform.Tizen;
#else
           => AppDevicePlatform.Unknown;
#endif
        public string DeviceName => DeviceInfo.Current.Name;
    }
}
