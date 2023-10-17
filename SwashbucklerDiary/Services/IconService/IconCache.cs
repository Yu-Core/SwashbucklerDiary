namespace SwashbucklerDiary.Services
{
    public static class IconCache
    {
        public static Dictionary<string, string> WeatherIcons { get; private set; } = new();

        public static Dictionary<string, string> MoodIcons { get; private set; } = new();

        public static void SetWeatherIcons(Dictionary<string, string> icons)
        {
            if(!WeatherIcons.Any())
            {
                WeatherIcons = icons;
            }
        }

        public static void SetMoodIcons(Dictionary<string, string> icons)
        {
            if (!MoodIcons.Any())
            {
                MoodIcons = icons;
            }
        }
    }
}
