using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Essentials
{
    public interface IPreferences
    {
        Task<bool> ContainsKey(string key);

        Task<T> Get<T>(Setting setting);

        Task<T> Get<T>(Setting setting, T defaultValue);

        Task<T> Get<T>(string key);

        Task<T> Get<T>(string key, T defaultValue);

        Task Set<T>(Setting setting, T value);

        Task Set<T>(string key, T value);

        Task Remove(string key);

        Task Remove(IEnumerable<string> keys);

        Task Remove(Setting setting);

        Task Clear();
    }
}
