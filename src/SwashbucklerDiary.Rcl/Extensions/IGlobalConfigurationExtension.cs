using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static class IGlobalConfigurationExtension
    {
        private readonly static string errorIcon = "close";

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

        public static string GetPlatformIcon(this IGlobalConfiguration globalConfiguration, AppPlatform appPlatform)
        {
            if (globalConfiguration.AppPlatformIcons.TryGetValue(appPlatform, out string? value))
            {
                return value;
            }

            if (globalConfiguration.AppPlatformIcons.TryGetValue(AppPlatform.Unknown, out string? value2))
            {
                return value2;
            }

            return errorIcon;
        }
    }
}
