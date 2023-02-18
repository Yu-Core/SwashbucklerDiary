using NoDecentDiary.IServices;

namespace NoDecentDiary.Services
{
    public class SettingsService : ISettingsService
    {
        public Task<bool> ContainsKey(string key)
        {
            var result = Preferences.Default.ContainsKey(key);
            return Task.FromResult(result);
        }

        public Task<T> Get<T>(string key, T defaultValue)
        {
            var result = Preferences.Default.Get(key, defaultValue);
            return Task.FromResult(result);
        }

        public Task Save<T>(string key, T value)
        {
            Preferences.Default.Set(key, value);
            return Task.CompletedTask;
        }

        public Task<string> GetLanguage()
        {
            return Get("Language", "zh-CN");
        }
    }
}
