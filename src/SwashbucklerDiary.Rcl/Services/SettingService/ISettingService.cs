using SwashbucklerDiary.Rcl.Essentials;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Services
{
    public interface ISettingService : IPreferences
    {
        event Action? SettingsChanged;

        T Get<T>(Expression<Func<Setting, T>> expr);

        T Get<T>(Expression<Func<Setting, T>> expr, T defaultValue);

        T Get<T>(string key);

        T Get<T>(string key, T defaultValue);

        Task RemoveAsync<T>(Expression<Func<Setting, T>> expr);

        Task SetAsync<T>(Expression<Func<Setting, T>> expr, T value);

        Task SetSettingsFromObjectAsync(Setting obj, Func<string, bool>? func = null);

        Setting SaveSettingsToObject(Func<string, bool>? func = null);

        T GetTemp<T>(Expression<Func<TempSetting, T>> expr);

        T GetTemp<T>(Expression<Func<TempSetting, T>> expr, T defaultValue);

        T GetTemp<T>(string key);

        T GetTemp<T>(string key, T defaultValue);

        void RemoveTemp(string key);

        void RemoveTemp<T>(Expression<Func<TempSetting, T>> expr);

        void SetTemp<T>(Expression<Func<TempSetting, T>> expr, T value);

        void SetTemp<T>(string key, T value);
    }
}
