using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;
using System.Text.Json;

namespace SwashbucklerDiary.Maui.Essentials
{
    public class Preferences : Rcl.Essentials.IPreferences
    {
        private readonly Lazy<Dictionary<Setting, object>> _defalutSettings;

        private Dictionary<Setting, object> DefalutSettings => _defalutSettings.Value;

        public Preferences(IStaticWebAssets staticWebAssets)
        {
            _defalutSettings = new(() => staticWebAssets.ReadJsonAsync<Dictionary<Setting, object>>("json/setting/settings.json").Result);
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

        public Task<T> Get<T>(Setting type)
        {
            if (DefalutSettings[type] is JsonElement element)
            {
                DefalutSettings[type] = element.Deserialize<T>() ?? default!;
            }

            return Get(type, (T)DefalutSettings[type]);
        }

        public Task<T> Get<T>(Setting type, T defaultValue)
        {
            var key = Enum.GetName(typeof(Setting), type);
            return Get(key!, defaultValue);
        }

        public Task Remove(string key)
        {
            Microsoft.Maui.Storage.Preferences.Default.Remove(key);
            return Task.CompletedTask;
        }

        public Task Remove(Setting type)
        {
            var key = Enum.GetName(typeof(Setting), type);
            return Remove(key!);
        }

        public Task Set<T>(string key, T value)
        {
            Microsoft.Maui.Storage.Preferences.Default.Set(key, value);
            return Task.CompletedTask;
        }

        public Task Set<T>(Setting type, T value)
        {
            var key = Enum.GetName(typeof(Setting), type);
            return Set(key!, value);
        }
    }
}
