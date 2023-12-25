using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public interface IIconService
    {
        Dictionary<string, string> GetWeatherIcons();

        Dictionary<string, string> GetMoodIcons();

        string GetWeatherIcon(string key);

        string GetMoodIcon(string key);

        string GetDeviceSystemIcon(AppDevicePlatform deviceSystem);
    }
}
