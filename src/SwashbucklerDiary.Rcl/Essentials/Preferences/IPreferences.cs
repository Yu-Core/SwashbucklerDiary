namespace SwashbucklerDiary.Rcl.Essentials
{
    public interface IPreferences
    {
        Task<bool> ContainsKey(string key);

        Task<T> GetAsync<T>(string key, T defaultValue);

        Task SetAsync<T>(string key, T value);

        Task RemoveAsync(string key);

        Task RemoveAsync(IEnumerable<string> keys);

        Task ClearAsync();
    }
}
