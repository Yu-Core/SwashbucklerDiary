using Blazored.LocalStorage;
using SwashbucklerDiary.Rcl.Essentials;
using System.Text.Json;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class Preferences : IPreferences
    {
        private readonly ILocalStorageService _localStorage;

        public Preferences(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        public virtual async Task ClearAsync()
        {
            await _localStorage.ClearAsync().ConfigureAwait(false);
        }

        public async Task<bool> ContainsKey(string key)
        {
            return await _localStorage.ContainKeyAsync(key).ConfigureAwait(false);
        }

        public virtual async Task<T> GetAsync<T>(string key, T defaultValue)
        {
            string? result = await _localStorage.GetItemAsStringAsync(key).ConfigureAwait(false);
            if (result is null)
            {
                return defaultValue;
            }

            T t = JsonSerializer.Deserialize<T>(result) ?? default!;
            return t;
        }

        public virtual async Task RemoveAsync(string key)
        {
            await _localStorage.RemoveItemAsync(key).ConfigureAwait(false);
        }

        public virtual async Task RemoveAsync(IEnumerable<string> keys)
        {
            await _localStorage.RemoveItemsAsync(keys).ConfigureAwait(false);
        }

        public virtual async Task SetAsync<T>(string key, T value)
        {
            string json = JsonSerializer.Serialize(value);
            await _localStorage.SetItemAsStringAsync(key, json).ConfigureAwait(false);
        }
    }
}
