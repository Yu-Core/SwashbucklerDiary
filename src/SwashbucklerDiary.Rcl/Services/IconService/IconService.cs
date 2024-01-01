using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public abstract class IconService : IIconService
    {
        private readonly static string errorIcon = "mdi-close";

        public abstract Dictionary<string, string> WeatherIcons { get; }

        public abstract Dictionary<string, string> MoodIcons { get; }

        public abstract Dictionary<AppDevicePlatform, string> DevicePlatformIcons { get; }

        public string GetWeatherIcon(string key)
        {
            if (string.IsNullOrEmpty(key) || !WeatherIcons.TryGetValue(key, out string? value))
            {
                return errorIcon;
            }

            return value;
        }

        public string GetMoodIcon(string key)
        {
            if (string.IsNullOrEmpty(key) || !MoodIcons.TryGetValue(key, out string? value))
            {
                return errorIcon;
            }

            return value;
        }

        public Dictionary<string, string> GetWeatherIcons()
        {
            return WeatherIcons;
        }

        public Dictionary<string, string> GetMoodIcons()
        {
            return MoodIcons;
        }

        public string GetDeviceSystemIcon(AppDevicePlatform deviceSystem)
        {
            if (!DevicePlatformIcons.TryGetValue(deviceSystem, out string? value))
            {
                return errorIcon;
            }

            return value;
        }
    }
}
