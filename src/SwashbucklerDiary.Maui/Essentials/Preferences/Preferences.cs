namespace SwashbucklerDiary.Maui.Essentials
{
    public class Preferences : Rcl.Essentials.IPreferences
    {
        public Task Clear()
        {
            Microsoft.Maui.Storage.Preferences.Default.Clear();
            return Task.CompletedTask;
        }

        public Task<bool> ContainsKey(string key)
        {
            var result = Microsoft.Maui.Storage.Preferences.Default.ContainsKey(key);
            return Task.FromResult(result);
        }

        public Task<T> Get<T>(string key, T defaultValue)
        {
            var result = Microsoft.Maui.Storage.Preferences.Default.Get(key, defaultValue);
            return Task.FromResult(result);
        }


        public Task Remove(string key)
        {
            Microsoft.Maui.Storage.Preferences.Default.Remove(key);
            return Task.CompletedTask;
        }

        public Task Remove(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                Microsoft.Maui.Storage.Preferences.Default.Remove(key);
            }
            return Task.CompletedTask;
        }

        public Task Set<T>(string key, T value)
        {
            Microsoft.Maui.Storage.Preferences.Default.Set(key, value);
            return Task.CompletedTask;
        }
    }
}
