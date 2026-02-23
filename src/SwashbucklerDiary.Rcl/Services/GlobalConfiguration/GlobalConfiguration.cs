using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public class GlobalConfiguration : IGlobalConfiguration
    {
        private readonly IStaticWebAssets _staticWebAssets;

        public Dictionary<string, string> WeatherIcons { get; set; } = [];

        public Dictionary<string, string> MoodIcons { get; set; } = [];

        public Dictionary<AppPlatform, string> AppPlatformIcons { get; set; } = [];

        public List<AchievementModel> Achievements { get; set; } = [];

        public GlobalConfiguration(IStaticWebAssets staticWebAssets)
        {
            _staticWebAssets = staticWebAssets;
        }

        public async Task InitializeAsync()
        {
            WeatherIcons = await _staticWebAssets.ReadJsonAsync<Dictionary<string, string>>("json/icon/weather-icons.json");
            MoodIcons = await _staticWebAssets.ReadJsonAsync<Dictionary<string, string>>("json/icon/mood-icons.json");
            AppPlatformIcons = await _staticWebAssets.ReadJsonAsync<Dictionary<AppPlatform, string>>("json/icon/app-platform-icons.json");
            Achievements = await _staticWebAssets.ReadJsonAsync<List<AchievementModel>>("json/achievement/achievements.json");
        }
    }
}
