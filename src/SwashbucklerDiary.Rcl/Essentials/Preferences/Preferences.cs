using SwashbucklerDiary.Shared;
using System.Text.Json;

namespace SwashbucklerDiary.Rcl.Essentials
{
    //TODO: 应该把Essentials中的内容单独一个项目，暂时没精力，以后再做
    public abstract class Preferences : Rcl.Essentials.IPreferences
    {
        private readonly Lazy<Task<Dictionary<string, object>>> _defalutSettings;

        public Preferences(IStaticWebAssets staticWebAssets)
        {
            _defalutSettings = new(() => staticWebAssets.ReadJsonAsync<Dictionary<string, object>>("json/setting/settings.json"));
        }

        public abstract Task<bool> ContainsKey(string key);

        public abstract Task<T> Get<T>(string key, T defaultValue);

        public abstract Task Remove(string key);

        public abstract Task Remove(IEnumerable<string> keys);

        public abstract Task Set<T>(string key, T value);

        public abstract Task Clear();

        public async Task<T> Get<T>(string key)
        {
            var defalutSettings = await _defalutSettings.Value.ConfigureAwait(false);
            bool contain = defalutSettings.TryGetValue(key, out var defaultValue);
            if (!contain)
            {
                throw new Exception("Missing defaultValue");
            }

            if (defaultValue is JsonElement element)
            {
                defalutSettings[key] = element.Deserialize<T>() ?? default!;
            }

            return await Get(key, (T)defalutSettings[key]).ConfigureAwait(false);
        }

        public Task<T> Get<T>(Setting setting)
        {
            var key = Enum.GetName(typeof(Setting), setting);
            return Get<T>(key!);
        }

        public Task<T> Get<T>(Setting setting, T defaultValue)
        {
            var key = Enum.GetName(typeof(Setting), setting);
            return Get(key!, defaultValue);
        }

        public Task Remove(Setting setting)
        {
            var key = Enum.GetName(typeof(Setting), setting);
            return Remove(key!);
        }

        public Task Set<T>(Setting setting, T value)
        {
            var key = Enum.GetName(typeof(Setting), setting);
            return Set(key!, value);
        }
    }
}
