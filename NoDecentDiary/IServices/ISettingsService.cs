namespace NoDecentDiary.IServices
{
    public interface ISettingsService
    {
        Task<T> Get<T>(string key, T defaultValue);
        Task Save<T>(string key, T value);
        Task<bool> ContainsKey(string key);
    }
}
