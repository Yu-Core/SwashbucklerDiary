namespace SwashbucklerDiary.Services
{
    public class IconService
    {
        private readonly string ErrorIcon = "mdi-close";
        public readonly Dictionary<string, string> WeatherIcon = new()
        {
            {"sunny","mdi-weather-sunny" },
            {"cloudy","mdi-weather-cloudy" },
            {"rainy","mdi-weather-rainy" },
            {"lightning-rainy","mdi-weather-lightning-rainy" },
            {"sleet","mdi-weather-snowy-rainy" },
            {"snow","mdi-snowflake" },
            {"windy","mdi-weather-windy" },
            {"fog","mdi-weather-fog" },
            {"dust","mdi-weather-dust" },
        };
        public readonly Dictionary<string, string> MoodIcon = new()
        {
            {"happy","mdi-emoticon-happy-outline" },
            {"excited","mdi-emoticon-excited-outline" },
            {"neutral","mdi-emoticon-neutral-outline" },
            {"cool","mdi-emoticon-cool-outline" },
            {"sad","mdi-emoticon-sad-outline" },
            {"lol","mdi-emoticon-lol-outline" },
            {"cry","mdi-emoticon-cry-outline" },
            {"kiss","mdi-emoticon-kiss-outline" },
            {"dead","mdi-emoticon-dead-outline" },
            {"angry","mdi-emoticon-angry-outline" },
            {"devil","mdi-emoticon-devil-outline" },
            {"confused","mdi-emoticon-confused-outline" },
            {"wink","mdi-emoticon-wink-outline" },
            {"poop","mdi-emoticon-poop-outline" },
            {"sick","mdi-emoticon-sick-outline" },
        };
        public string GetWeatherIcon(string key)
        {
            if (string.IsNullOrEmpty(key) || !WeatherIcon.ContainsKey(key))
            {
                return ErrorIcon;
            }
            return WeatherIcon[key];
        }
        public string GetMoodIcon(string key)
        {
            if (string.IsNullOrEmpty(key) || !MoodIcon.ContainsKey(key))
            {
                return ErrorIcon;
            }
            return MoodIcon[key];
        }
    }
}
