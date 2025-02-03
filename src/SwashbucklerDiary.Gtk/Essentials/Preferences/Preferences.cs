
namespace SwashbucklerDiary.Gtk.Essentials
{
    public class Preferences : Rcl.Essentials.IPreferences
    {
        private static readonly UnpackagedPreferencesImplementation? _preferences;
        public static UnpackagedPreferencesImplementation Default => _preferences ?? new();

        public Task ClearAsync()
        {
            Default.Clear();
            return Task.CompletedTask;
        }

        public Task<bool> ContainsKey(string key)
        {
            var result = Default.ContainsKey(key);
            return Task.FromResult(result);
        }

        public Task<T> GetAsync<T>(string key, T defaultValue)
        {
            var result = Default.Get<T>(key, defaultValue);
            return Task.FromResult(result);
        }

        public Task RemoveAsync(string key)
        {
            Default.Remove(key);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                Default.Remove(key);
            }
            return Task.CompletedTask;
        }

        public Task SetAsync<T>(string key, T value)
        {
            Default.Set(key, value);
            return Task.CompletedTask;
        }
    }
}
