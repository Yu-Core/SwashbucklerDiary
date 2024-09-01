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

        private readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        private Dictionary<string, object> settings = [];

        public SettingService(IPreferences preferences,
            IStaticWebAssets staticWebAssets,
            IJSRuntime jSRuntime) :
            base(preferences)
        {
            jsonSerializerOptions.Converters.Add(new ObjectToInferredTypesConverter());
            _module = new(() => ((IJSInProcessRuntime)jSRuntime).ImportJsModule("js/setting.js"));
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            settings = await ReadSettings();
        }

        public override T Get<T>(string key)
        {
            if (settings.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            if (_defalutSettings.TryGetValue(key, out var defaulValue))
            {
                return (T)defaulValue;
            }

            return default!;
        }

        public override T Get<T>(string key, T defaultValue)
        {
            if (settings.TryGetValue(key, out var settingValue))
            {
                return (T)settingValue;
            }

            return defaultValue;
        }

        public override Task SetAsync<T>(string key, T value)
        {
            settings[key] = value;
            return base.SetAsync(key, value);
        }

        public override async Task<T> GetAsync<T>(string key, T defaultValue)
        {
            var value = await base.GetAsync<T>(key, defaultValue);
            settings[key] = value;
            return value;
        }

        public override Task RemoveAsync(string key)
        {
            settings.Remove(key);
            return base.RemoveAsync(key);
        }

        public override Task RemoveAsync(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                settings.Remove(key);
            }

            return base.RemoveAsync(keys);
        }

        public override Task ClearAsync()
        {
            settings.Clear();
            return base.ClearAsync();
        }


        private async Task<Dictionary<string, object>> ReadSettings()
        {
            List<string> keys = _defalutSettings.Keys.ToList();

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
