using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;
using SwashbucklerDiary.WebAssembly.Services;

namespace SwashbucklerDiary.WebAssembly.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static async Task<IServiceCollection> AddGlobalAsync(this IServiceCollection services)
        {
            var staticWebAssets = services.BuildServiceProvider().GetRequiredService<IStaticWebAssets>();

            var weatherIcons = await staticWebAssets.ReadJsonAsync<Dictionary<string, string>>("json/icon/weather-icons.json");
            services.AddKeyedSingleton(nameof(IconService.WeatherIcons), weatherIcons);

            var moodIcons = await staticWebAssets.ReadJsonAsync<Dictionary<string, string>>("json/icon/mood-icons.json");
            services.AddKeyedSingleton(nameof(IconService.MoodIcons), moodIcons);

            var devicePlatformIcons = await staticWebAssets.ReadJsonAsync<Dictionary<AppDevicePlatform, string>>("json/icon/device-platform-icons.json");
            services.AddKeyedSingleton(nameof(IconService.DevicePlatformIcons), devicePlatformIcons);

            var achievements = await staticWebAssets.ReadJsonAsync<Dictionary<Achievement, int[]>>("json/achievement/achievements.json");
            services.AddKeyedSingleton("Achievements", achievements);

            var languages = await staticWebAssets.ReadJsonAsync<Dictionary<string, string>>("json/i18n/languages.json");
            services.AddKeyedSingleton(nameof(I18nService.Languages), languages);

            return services;
        }
    }
}
