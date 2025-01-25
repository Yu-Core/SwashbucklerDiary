
namespace SwashbucklerDiary.Gtk.Essentials
{
    public class Preferences : Rcl.Essentials.IPreferences
    {
        protected readonly UnpackagedPreferencesImplementation _preferences;
        public Preferences(UnpackagedPreferencesImplementation preferences)
        {
            _preferences = preferences;
        }

        public Task ClearAsync()
        {
            _preferences.Clear();
            return Task.CompletedTask;
        }

        public Task<bool> ContainsKey(string key)
        {
            var result = _preferences.ContainsKey(key);
            return Task.FromResult(result);
        }

        public Task<T> GetAsync<T>(string key, T defaultValue)
        {
            var result = _preferences.Get<T>(key, defaultValue);
            return Task.FromResult(result);
        }

        public Task RemoveAsync(string key)
        {
            _preferences.Remove(key);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                _preferences.Remove(key);
            }
            return Task.CompletedTask;
        }

        public Task SetAsync<T>(string key, T value)
        {
            _preferences.Set(key, value);
            return Task.CompletedTask;
        }
    }
}
