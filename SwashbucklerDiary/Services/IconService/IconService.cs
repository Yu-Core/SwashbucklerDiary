using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary.Services
{
    public class IconService : IIconService
    {
        private readonly static string ErrorIcon = "mdi-close";
        
        public string GetWeatherIcon(string key)
        {
            if (string.IsNullOrEmpty(key) || !IconCache.WeatherIcons.TryGetValue(key, out string? value))
            {
                return ErrorIcon;
            }

            return value;
        }

        public string GetMoodIcon(string key)
        {
            if (string.IsNullOrEmpty(key) || !IconCache.MoodIcons.TryGetValue(key, out string? value))
            {
                return ErrorIcon;
            }

            return value;
        }

        public Dictionary<string, string> GetWeatherIcons()
        {
            return IconCache.WeatherIcons;
        }

        public Dictionary<string, string> GetMoodIcons()
        {
            return IconCache.MoodIcons;
        }
    }
}
