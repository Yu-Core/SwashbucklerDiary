using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Services
{
    public class SettingsService : ISettingsService
    {
        private readonly Dictionary<SettingType, dynamic> Settings = new()
        {
            {SettingType.Language,"zh-CN" },
            {SettingType.Title,false },
            {SettingType.Markdown,true },
            {SettingType.UserName,"" },
            {SettingType.Sign,"" },
            {SettingType.Avatar,"./logo/logo.svg" },
            {SettingType.Privacy,true },
            {SettingType.PrivatePassword,"" },
            {SettingType.BackupsPath,"" },
        };
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

        public async Task<dynamic> Get(SettingType type)
        {
            var defaultValue = Settings[type];
            return await Get(type, defaultValue);
        }

        public Task<T> Get<T>(SettingType type)
        {
            var defaultValue = Settings[type];
            return Get(type, defaultValue);
        }

        public Task<T> Get<T>(SettingType type, T defaultValue)
        {
            var key = Enum.GetName(typeof(SettingType), type);
            return Get(key!, defaultValue);
        }

        public Task Save<T>(string key, T value)
        {
            Preferences.Default.Set(key, value);
            return Task.CompletedTask;
        }

        public Task Save<T>(SettingType type, T value)
        {
            var key = Enum.GetName(typeof(SettingType), type);
            return Save(key!, value);
        }
    }
}
