using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static class IGlobalConfigurationExtension
    {
        private readonly static string errorIcon = "mdi-close";

        public static string GetWeatherIcon(this IGlobalConfiguration globalConfiguration, string key)
        {
            if (string.IsNullOrEmpty(key) || !globalConfiguration.WeatherIcons.TryGetValue(key, out string? value))
            {
                return errorIcon;
            }

            return value;
        }

        public static string GetMoodIcon(this IGlobalConfiguration globalConfiguration, string key)
        {
            if (string.IsNullOrEmpty(key) || !globalConfiguration.MoodIcons.TryGetValue(key, out string? value))
            {
                return errorIcon;
            }

            return value;
        }

        public static string GetPlatformIcon(this IGlobalConfiguration globalConfiguration, AppDevicePlatform devicePlatform)
        {
            if (!globalConfiguration.DevicePlatformIcons.TryGetValue(devicePlatform, out string? value))
            {
                return errorIcon;
            }

            return value;
        }
    }
}
