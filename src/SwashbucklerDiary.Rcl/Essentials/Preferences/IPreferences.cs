namespace SwashbucklerDiary.Rcl.Essentials
{
    public interface IPreferences
    {
        Task<bool> ContainsKey(string key);

        Task<T> Get<T>(string key, T defaultValue);

        Task Set<T>(string key, T value);

        Task Remove(string key);

        Task Remove(IEnumerable<string> keys);

        Task Clear();
    }
}
