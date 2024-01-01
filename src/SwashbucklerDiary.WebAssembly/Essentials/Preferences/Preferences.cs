using Blazored.LocalStorage;
using SwashbucklerDiary.Rcl.Essentials;
using System.Text.Json;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class Preferences : Rcl.Essentials.Preferences
    {
        private readonly ISyncLocalStorageService _localStorage;

        public Preferences(IStaticWebAssets staticWebAssets,
            ISyncLocalStorageService localStorage) : base(staticWebAssets)
        {
            _localStorage = localStorage;
        }

        public override Task<bool> ContainsKey(string key)
        {
            var result = _localStorage.ContainKey(key);
            return Task.FromResult(result);
        }

        public override Task<T> Get<T>(string key, T defaultValue)
        {
            string result = _localStorage.GetItemAsString(key);
            if(result is null)
            {
                return Task.FromResult(defaultValue);
            }

            T t = JsonSerializer.Deserialize<T>(result) ?? default!;
            return Task.FromResult(t);
        }

        public override Task Remove(string key)
        {
            _localStorage.RemoveItem(key);
            return Task.CompletedTask;
        }

        public override Task Set<T>(string key, T value)
        {
            string json = JsonSerializer.Serialize(value);
            _localStorage.SetItemAsString(key, json);
            return Task.CompletedTask;
        }
    }
}
