using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using System.Text.Json;

namespace SwashbucklerDiary.WebAssembly.Services
{
    public class SettingService : Rcl.Services.SettingService, ISettingService
    {
        private readonly Lazy<ValueTask<IJSObjectReference>> _module;

        private readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = {
                new ObjectToInferredTypesConverter()
            }
        };

        private Dictionary<string, object> settings = [];

        public SettingService(IPreferences preferences,
            IJSRuntime jSRuntime) :
            base(preferences)
        {
            _module = new(() => jSRuntime.ImportJsModule("js/setting.js"));
        }

        public async Task InitializeAsync()
        {
            settings = await ReadSettings().ConfigureAwait(false);
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
            var value = await base.GetAsync<T>(key, defaultValue).ConfigureAwait(false);
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

            var module = await _module.Value.ConfigureAwait(false);
            var serialisedData = await module.InvokeAsync<string>("readSettings", keys).ConfigureAwait(false);
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
