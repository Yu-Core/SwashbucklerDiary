using Blazored.LocalStorage;
using SwashbucklerDiary.Rcl.Essentials;
using System.Text.Json;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class Preferences : IPreferences
    {
        private readonly ISyncLocalStorageService _localStorage;

        public Preferences(ISyncLocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public virtual Task ClearAsync()
        {
            _localStorage.Clear();
            return Task.CompletedTask;
        }

        public Task<bool> ContainsKey(string key)
        {
            var result = _localStorage.ContainKey(key);
            return Task.FromResult(result);
        }

        public virtual Task<T> GetAsync<T>(string key, T defaultValue)
        {
            string result = _localStorage.GetItemAsString(key);
            if (result is null)
            {
                return Task.FromResult(defaultValue);
            }

            T t = JsonSerializer.Deserialize<T>(result) ?? default!;
            return Task.FromResult(t);
        }

        public virtual Task RemoveAsync(string key)
        {
            _localStorage.RemoveItem(key);
            return Task.CompletedTask;
        }

        public virtual Task RemoveAsync(IEnumerable<string> keys)
        {
            _localStorage.RemoveItems(keys);
            return Task.CompletedTask;
        }

        public virtual Task SetAsync<T>(string key, T value)
        {
            string json = JsonSerializer.Serialize(value);
            _localStorage.SetItemAsString(key, json);
            return Task.CompletedTask;
        }
    }
}
