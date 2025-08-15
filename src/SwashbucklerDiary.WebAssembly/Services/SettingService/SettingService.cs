using Blazored.LocalStorage;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using SwashbucklerDiary.WebAssembly.Essentials;
using SwashbucklerDiary.WebAssembly.Extensions;
using System.Text.Json;

namespace SwashbucklerDiary.WebAssembly.Services
{
    public class SettingService : Preferences, ISettingService
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

        public Dictionary<string, object> DefalutSettings { get; set; } = [];
        public Dictionary<string, object> TempSettings { get; set; } = [];
        public Action? SettingsChanged { get; set; }

        public SettingService(ILocalStorageService localStorage,
            IJSRuntime jSRuntime) :
            base(localStorage)
        {
            _module = new(() => jSRuntime.ImportJsModule("js/setting.js"));
        }

        public async Task InitializeAsync()
        {
            await ISettingService.DefalutInitializeAsync(DefalutSettings).ConfigureAwait(false);
            settings = await ReadSettings().ConfigureAwait(false);
        }

        public T Get<T>(string key)
        {
            if (settings.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            if (DefalutSettings.TryGetValue(key, out var defaulValue))
            {
                return (T)defaulValue;
            }

            return default!;
        }

        public T Get<T>(string key, T defaultValue)
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
            List<string> keys = DefalutSettings.Keys.ToList();

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
