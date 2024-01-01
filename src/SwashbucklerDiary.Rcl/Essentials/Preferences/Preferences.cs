using SwashbucklerDiary.Shared;
using System.Text.Json;

namespace SwashbucklerDiary.Rcl.Essentials
{
    public abstract class Preferences : Rcl.Essentials.IPreferences
    {
        private readonly Lazy<Task<Dictionary<Setting, object>>> _defalutSettings;

        public Preferences(IStaticWebAssets staticWebAssets)
        {
            _defalutSettings = new(() => staticWebAssets.ReadJsonAsync<Dictionary<Setting, object>>("json/setting/settings.json"));
        }

        public abstract Task<bool> ContainsKey(string key);

        public abstract Task<T> Get<T>(string key, T defaultValue);

        public abstract Task Remove(string key);

        public abstract Task Set<T>(string key, T value);

        public async Task<T> Get<T>(Setting type)
        {
            var defalutSettings = await _defalutSettings.Value.ConfigureAwait(false);
            if (defalutSettings[type] is JsonElement element)
            {
                defalutSettings[type] = element.Deserialize<T>() ?? default!;
            }

            return await Get(type, (T)defalutSettings[type]).ConfigureAwait(false);
        }

        public Task<T> Get<T>(Setting type, T defaultValue)
        {
            var key = Enum.GetName(typeof(Setting), type);
            return Get(key!, defaultValue);
        }

        public Task Remove(Setting type)
        {
            var key = Enum.GetName(typeof(Setting), type);
            return Remove(key!);
        }

        public Task Set<T>(Setting type, T value)
        {
            var key = Enum.GetName(typeof(Setting), type);
            return Set(key!, value);
        }
    }
}
