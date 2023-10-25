using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        private readonly Dictionary<DevicePlatformType, string> DeviceIcons = new()
        {
            { DevicePlatformType.Windows, "mdi-microsoft-windows" },
            { DevicePlatformType.Android, "mdi-android" },
            { DevicePlatformType.iOS, "mdi-apple-ios" },
            { DevicePlatformType.MacOS, "mdi-apple" },
            { DevicePlatformType.Unknown, "mdi-monitor-cellphone" },
        };

        public DevicePlatformType GetDevicePlatformType()
        {
#if WINDOWS
            return DevicePlatformType.Windows;
#elif ANDROID
            return DevicePlatformType.Android;
#elif MACCATALYST
            return DevicePlatformType.MacOS;
#elif IOS
            return DevicePlatformType.iOS;
#else
            return DevicePlatformType.Unknown;
#endif
        }

        public string GetDevicePlatformTypeIcon(DevicePlatformType platformType)
        {
            if (DeviceIcons.TryGetValue(platformType, out string? value))
            {
                return value;
            }
            else
            {
                return DeviceIcons[DevicePlatformType.Unknown];
            }
        }
    }
}
