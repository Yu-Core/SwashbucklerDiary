using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;
using SwashbucklerDiary.WebAssembly.Extensions;
using System.Text.Json;

namespace SwashbucklerDiary.WebAssembly.Services
{
    public class SettingService : Rcl.Services.SettingService
    {
        private readonly Lazy<ValueTask<IJSInProcessObjectReference>> _module;

        private Dictionary<string, object> settings = [];

        public SettingService(IPreferences preferences,
            IStaticWebAssets staticWebAssets,
            IJSRuntime jSRuntime) :
            base(preferences, staticWebAssets)
        {
            _module = new(() => ((IJSInProcessRuntime)jSRuntime).ImportJsModule("js/setting.js"));

        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            settings = await ReadSettings();
        }

        public override T Get<T>(Setting setting)
        {
            var key = setting.ToString();
            if (settings.TryGetValue(key, out var settingValue))
            {
                return (T)settingValue;
            }

            if (defalutSettings.TryGetValue(key, out var defaulSettingtValue))
            {
                return (T)defaulSettingtValue;
            }

            return default!;
        }

        public override T Get<T>(Setting setting, T defaultValue)
        {
            var key = setting.ToString();
            if (settings.TryGetValue(key, out var settingValue))
            {
                return (T)settingValue;
            }

            return defaultValue;
        }

        public override Task Set<T>(Setting setting, T value)
        {
            var key = setting.ToString();
            settings[key] = value;
            return Set(key, value);
        }

        private async Task<Dictionary<string, object>> ReadSettings()
        {
            List<string> keys = [];
            foreach (Setting item in Enum.GetValues(typeof(Setting)))
            {
                var key = item.ToString();
                keys.Add(key);
            }

            var module = await _module.Value;
            var serialisedData = module.Invoke<string>("readSettings", keys);
            try
            {
                return JsonSerializer.Deserialize<Dictionary<string, object>>(serialisedData, jsonSerializerOptions) ?? [];
            }
            catch (Exception)
            {
                return [];
            }

        }
    }
}
