namespace SwashbucklerDiary.Maui.Essentials
{
    public class Preferences : Rcl.Essentials.IPreferences
    {
        public Task ClearAsync()
        {
            Microsoft.Maui.Storage.Preferences.Default.Clear();
            return Task.CompletedTask;
        }

        public Task<bool> ContainsKey(string key)
        {
            var result = Microsoft.Maui.Storage.Preferences.Default.ContainsKey(key);
            return Task.FromResult(result);
        }

        public Task<T> GetAsync<T>(string key, T defaultValue)
        {
            var result = Microsoft.Maui.Storage.Preferences.Default.Get(key, defaultValue);
            return Task.FromResult(result);
        }


        public Task RemoveAsync(string key)
        {
            Microsoft.Maui.Storage.Preferences.Default.Remove(key);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                Microsoft.Maui.Storage.Preferences.Default.Remove(key);
            }
            return Task.CompletedTask;
        }

        public Task SetAsync<T>(string key, T value)
        {
            Microsoft.Maui.Storage.Preferences.Default.Set(key, value);
            return Task.CompletedTask;
        }
    }
}
