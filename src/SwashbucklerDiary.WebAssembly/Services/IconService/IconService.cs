using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.WebAssembly.Services
{
    public class IconService : Rcl.Services.IconService
    {
        private readonly Dictionary<string, string> _weatherIcons;

        private readonly Dictionary<string, string> _moodIcons;

        private readonly Dictionary<AppDevicePlatform, string> _devicePlatformIcons;

        public override Dictionary<string, string> WeatherIcons => _weatherIcons;

        public override Dictionary<string, string> MoodIcons => _moodIcons;

        public override Dictionary<AppDevicePlatform, string> DevicePlatformIcons => _devicePlatformIcons;

        public IconService([FromKeyedServices(nameof(WeatherIcons))] Dictionary<string, string> weatherIcons,
            [FromKeyedServices(nameof(MoodIcons))] Dictionary<string, string> moodIcons,
            [FromKeyedServices(nameof(DevicePlatformIcons))] Dictionary<AppDevicePlatform, string> devicePlatformIcons)
        {
            _weatherIcons = weatherIcons;
            _moodIcons = moodIcons;
            _devicePlatformIcons = devicePlatformIcons;
        }

    }
}
