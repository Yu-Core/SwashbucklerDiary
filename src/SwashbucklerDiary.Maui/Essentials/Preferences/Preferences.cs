namespace SwashbucklerDiary.Maui.Essentials
{
    public class Preferences : Rcl.Essentials.IPreferences
    {
        private readonly Microsoft.Maui.Storage.IPreferences _references;

        public Preferences()
        {
            _references = Microsoft.Maui.Storage.Preferences.Default;
        }

        public Task ClearAsync()
        {
            _references.Clear();
            return Task.CompletedTask;
        }

        public Task<bool> ContainsKey(string key)
        {
            var result = _references.ContainsKey(key);
            return Task.FromResult(result);
        }

        public Task<T> GetAsync<T>(string key, T defaultValue)
        {
            var result = _references.Get(key, defaultValue);
            return Task.FromResult(result);
        }


        public Task RemoveAsync(string key)
        {
            _references.Remove(key);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                _references.Remove(key);
            }
            return Task.CompletedTask;
        }

        public Task SetAsync<T>(string key, T value)
        {
            _references.Set(key, value);
            return Task.CompletedTask;
        }
    }
}
