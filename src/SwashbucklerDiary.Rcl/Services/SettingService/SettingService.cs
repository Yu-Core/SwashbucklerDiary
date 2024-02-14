using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;
using System.Text.Json;

namespace SwashbucklerDiary.Rcl.Services
{
    public class SettingService : ISettingService
    {
        private readonly IPreferences _preferences;

        private readonly Lazy<Task<Dictionary<string, object>>> _defalutSettings;

        public SettingService(IPreferences preferences, IStaticWebAssets staticWebAssets)
        {
            _preferences = preferences;
            _defalutSettings = new(() => staticWebAssets.ReadJsonAsync<Dictionary<string, object>>("json/setting/settings.json"));
        }

        public Task<bool> ContainsKey(string key)
            => _preferences.ContainsKey(key);

        public Task<T> Get<T>(string key, T defaultValue)
            => _preferences.Get(key, defaultValue);

        public Task Remove(string key)
            => _preferences.Remove(key);

        public Task Remove(IEnumerable<string> keys)
            => _preferences.Remove(keys);

        public Task Set<T>(string key, T value)
            => _preferences.Set(key, value);

        public Task Clear()
            => _preferences.Clear();

        public async Task<T> Get<T>(Setting setting)
        {
            var key = setting.ToString();
            var defalutSettings = await _defalutSettings.Value.ConfigureAwait(false);
            bool contain = defalutSettings.TryGetValue(key, out var defaultValue);
            if (!contain)
            {
                return default!;
            }

            if (defaultValue is JsonElement element)
            {
                defalutSettings[key] = element.Deserialize<T>() ?? default!;
            }

            return await Get(key, (T)defalutSettings[key]).ConfigureAwait(false);
        }

        public Task<T> Get<T>(Setting setting, T defaultValue)
        {
            var key = setting.ToString();
            return Get(key, defaultValue);
        }

        public Task Remove(Setting setting)
        {
            var key = setting.ToString();
            return Remove(key);
        }

        public Task Set<T>(Setting setting, T value)
        {
            var key = setting.ToString();
            return Set(key, value);
        }
    }
}
