using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public interface IGlobalConfiguration
    {
        Dictionary<string, string> WeatherIcons { get; set; }

        Dictionary<string, string> MoodIcons { get; set; }

        Dictionary<AppPlatform, string> AppPlatformIcons { get; set; }

        List<AchievementModel> Achievements { get; set; }

        Task InitializeAsync();
    }
}
