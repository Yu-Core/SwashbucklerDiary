using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        public AppDevicePlatform CurrentPlatform
#if WINDOWS
           => AppDevicePlatform.Windows;
#elif ANDROID
           => AppDevicePlatform.Android;
#elif MACCATALYST
           => AppDevicePlatform.macOS;
#elif IOS
           => AppDevicePlatform.iOS;
#elif TIZEN
           => AppDevicePlatform.Tizen;
#else
           => AppDevicePlatform.Unknown;
#endif
        public string DeviceName => DeviceInfo.Current.Name;
    }
}
