using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public class IconService : IIconService
    {
        private readonly static string errorIcon = "mdi-close";

        private readonly Lazy<Dictionary<string, string>> weatherIcons;

        private readonly Lazy<Dictionary<string, string>> moodIcons;

        private readonly Lazy<Dictionary<AppDevicePlatform, string>> deviceSystemIcons;

        public IconService(IStaticWebAssets staticWebAssets)
        {
            weatherIcons = new(()=> staticWebAssets.ReadJsonAsync<Dictionary<string, string>>("json/icon/mood-icons.json").Result);
            moodIcons = new(() => staticWebAssets.ReadJsonAsync<Dictionary<string, string>>("json/icon/weather-icons.json").Result) ;
            deviceSystemIcons = new(() => staticWebAssets.ReadJsonAsync<Dictionary<AppDevicePlatform, string>>("json/icon/device-system-icons.json").Result);
        }

        public string GetWeatherIcon(string key)
        {
            if (string.IsNullOrEmpty(key) || !weatherIcons.Value.TryGetValue(key, out string? value))
            {
                return errorIcon;
            }

            return value;
        }

        public string GetMoodIcon(string key)
        {
            if (string.IsNullOrEmpty(key) || !moodIcons.Value.TryGetValue(key, out string? value))
            {
                return errorIcon;
            }

            return value;
        }

        public Dictionary<string, string> GetWeatherIcons()
        {
            return weatherIcons.Value;
        }

        public Dictionary<string, string> GetMoodIcons()
        {
            return moodIcons.Value;
        }

        public string GetDeviceSystemIcon(AppDevicePlatform deviceSystem)
        {
            if (!deviceSystemIcons.Value.TryGetValue(deviceSystem, out string? value))
            {
                return errorIcon;
            }

            return value;
        }
    }
}
