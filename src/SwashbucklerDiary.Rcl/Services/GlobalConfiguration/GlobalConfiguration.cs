using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public class GlobalConfiguration : IGlobalConfiguration
    {
        private readonly IStaticWebAssets _staticWebAssets;

        public Dictionary<string, string> WeatherIcons { get; set; } = [];

        public Dictionary<string, string> MoodIcons { get; set; } = [];

        public Dictionary<AppDevicePlatform, string> DevicePlatformIcons { get; set; } = [];

        public Dictionary<string, string> Languages { get; set; } = [];

        public List<AchievementModel> Achievements { get; set; } = [];

        public GlobalConfiguration(IStaticWebAssets staticWebAssets)
        {
            _staticWebAssets = staticWebAssets;
        }

        public async Task InitializeAsync()
        {
            WeatherIcons = await _staticWebAssets.ReadJsonAsync<Dictionary<string, string>>("json/icon/weather-icons.json");
            MoodIcons = await _staticWebAssets.ReadJsonAsync<Dictionary<string, string>>("json/icon/mood-icons.json");
            DevicePlatformIcons = await _staticWebAssets.ReadJsonAsync<Dictionary<AppDevicePlatform, string>>("json/icon/device-platform-icons.json");
            Languages = await _staticWebAssets.ReadJsonAsync<Dictionary<string, string>>("json/i18n/languages.json");
            var achievementDictionary = await _staticWebAssets.ReadJsonAsync<Dictionary<Achievement, int[]>>("json/achievement/achievements.json");
            Achievements = achievementDictionary.SelectMany(item => item.Value, (item, count) => new AchievementModel(item.Key, count)).ToList();
        }
    }
}
