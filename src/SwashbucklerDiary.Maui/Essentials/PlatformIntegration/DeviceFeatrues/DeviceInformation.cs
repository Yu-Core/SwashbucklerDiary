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
           => AppDevicePlatform.MacOS;
#elif IOS
           => AppDevicePlatform.iOS;
#elif TIZEN
           => AppDevicePlatform.Tizen;
#else
           => AppDevicePlatform.Unknown;
#endif
    }
}
