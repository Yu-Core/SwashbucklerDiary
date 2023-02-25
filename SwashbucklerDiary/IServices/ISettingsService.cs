namespace SwashbucklerDiary.IServices
{
    public interface ISettingsService
    {
        Task<T> Get<T>(string key, T defaultValue);
        Task Save<T>(string key, T value);
        Task<bool> ContainsKey(string key);
        Task<string> GetLanguage();
        Task<bool> GetPrivacy();
    }
}
