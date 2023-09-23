using SwashbucklerDiary.Services;
using System.Text.Json;

namespace SwashbucklerDiary.Extensions
{
    public static partial class ServiceCollectionExtensions
    {
        public static IServiceCollection AddIconCache(this IServiceCollection services)
        {
            var weatherIconPath = "wwwroot/json/icon/weather-icon.json";
            var moodIconPath = "wwwroot/json/icon/mood-icon.json";
            var weatherIcons = ReadJsonFileAsync<Dictionary<string, string>>(weatherIconPath);
            var moodIcons = ReadJsonFileAsync<Dictionary<string, string>>(moodIconPath);
            IconCache.SetWeatherIcons(weatherIcons);
            IconCache.SetMoodIcons(moodIcons);
            return services;
        }

        public static T ReadJsonFileAsync<T>(string path)
        {
            bool exists = FileSystem.AppPackageFileExistsAsync(path).Result;
            if (!exists)
            {
                return default!;
            }

            using var stream = FileSystem.OpenAppPackageFileAsync(path).Result;
            using var reader = new StreamReader(stream);
            var contents = reader.ReadToEnd();
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<T>(contents, options) ?? throw new("not find json file");
        }
    }
}
