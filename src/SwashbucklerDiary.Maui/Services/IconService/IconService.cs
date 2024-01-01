using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Services
{
    public class IconService : Rcl.Services.IconService
    {
        private readonly Lazy<Dictionary<string, string>> _weatherIcons;

        private readonly Lazy<Dictionary<string, string>> _moodIcons;

        private readonly Lazy<Dictionary<AppDevicePlatform, string>> _devicePlatformIcons;

        public override Dictionary<string, string> WeatherIcons => _weatherIcons.Value;

        public override Dictionary<string, string> MoodIcons => _moodIcons.Value;

        public override Dictionary<AppDevicePlatform, string> DevicePlatformIcons => _devicePlatformIcons.Value;

        public IconService(IStaticWebAssets staticWebAssets)
        {
            _weatherIcons = new(()=> staticWebAssets.ReadJsonAsync<Dictionary<string, string>>("json/icon/weather-icons.json").Result);
            _moodIcons = new(() => staticWebAssets.ReadJsonAsync<Dictionary<string, string>>("json/icon/mood-icons.json").Result) ;
            _devicePlatformIcons = new(() => staticWebAssets.ReadJsonAsync<Dictionary<AppDevicePlatform, string>>("json/icon/device-platform-icons.json").Result);
        }

    }
}
