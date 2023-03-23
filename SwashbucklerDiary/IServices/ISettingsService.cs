using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.IServices
{
    public interface ISettingsService
    {
        Task InitDefault<T>(SettingType type);
        T GetDefault<T>(SettingType type);
        Task<dynamic> Get(SettingType type);
        Task<T> Get<T>(SettingType type);
        Task<T> Get<T>(SettingType type, T defaultValue);
        Task<T> Get<T>(string key, T defaultValue);
        Task Save<T>(SettingType type, T value);
        Task Save<T>(string key, T value);
        Task<bool> ContainsKey(string key);
    }
}
