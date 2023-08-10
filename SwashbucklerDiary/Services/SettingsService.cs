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
            {SettingType.UserName,string.Empty },
            {SettingType.Sign,string.Empty },
            {SettingType.Avatar,"./logo/logo.svg" },
            {SettingType.PrivacyMode,true },
            {SettingType.PrivatePassword,string.Empty },
            {SettingType.BackupsPath,string.Empty },
            {SettingType.ThemeState,(int)ThemeState.System },
            {SettingType.FirstSetLanguage,false },
            {SettingType.FirstAgree,false },
            {SettingType.WebDAVServerAddress,string.Empty },
            {SettingType.WebDAVAccount,string.Empty },
            {SettingType.WebDAVPassword,string.Empty },
            {SettingType.WelcomeText,true },
            {SettingType.Date,true },
            {SettingType.DiaryCardIcon,false },
            {SettingType.EditCreateTime,false },
            {SettingType.AlertTimeout,5000 },
            {SettingType.WebDAVCopyResources,false },

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
