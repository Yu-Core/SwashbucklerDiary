namespace SwashbucklerDiary.IServices
{
    public interface IIconService
    {
        Dictionary<string, string> GetWeatherIcons();

        Dictionary<string, string> GetMoodIcons();

        string GetWeatherIcon(string key);

        string GetMoodIcon(string key);
    }
}
