using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SwashbucklerDiary.Rcl.Services
{
    public abstract class SettingService : ISettingService
    {
        private readonly IPreferences _preferences;

        private readonly IStaticWebAssets _staticWebAssets;

        protected readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        protected Dictionary<string, object> defalutSettings = [];

        public SettingService(IPreferences preferences, IStaticWebAssets staticWebAssets)
        {
            _preferences = preferences;
            _staticWebAssets = staticWebAssets;
            jsonSerializerOptions.Converters.Add(new ObjectToInferredTypesConverter());
        }

        public virtual async Task InitializeAsync()
        {
            defalutSettings = await _staticWebAssets.ReadJsonAsync<Dictionary<string, object>>("json/setting/settings.json", true, jsonSerializerOptions);
        }

        public abstract T Get<T>(Setting setting);

        public abstract T Get<T>(Setting setting, T defaultValue);

        public abstract Task Set<T>(Setting setting, T value);

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

        public Task Remove(Setting setting)
        {
            var key = setting.ToString();
            return Remove(key);
        }

        public class ObjectToInferredTypesConverter : JsonConverter<object>
        {
            public override object Read(
                ref Utf8JsonReader reader,
                Type typeToConvert,
                JsonSerializerOptions options) => reader.TokenType switch
                {
                    JsonTokenType.True => true,
                    JsonTokenType.False => false,
                    JsonTokenType.Number when reader.TryGetInt32(out int i) => i,
                    JsonTokenType.Number => reader.GetDouble(),
                    JsonTokenType.String when reader.TryGetDateTime(out DateTime datetime) => datetime,
                    JsonTokenType.String => reader.GetString()!,
                    _ => JsonDocument.ParseValue(ref reader).RootElement.Clone()
                };

            public override void Write(
                Utf8JsonWriter writer,
                object objectToWrite,
                JsonSerializerOptions options) =>
                JsonSerializer.Serialize(writer, objectToWrite, objectToWrite.GetType(), options);
        }
    }
}
