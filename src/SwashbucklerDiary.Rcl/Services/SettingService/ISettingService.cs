using SwashbucklerDiary.Rcl.Essentials;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Services
{
    public interface ISettingService : IPreferences
    {
        Task InitializeAsync();

        T Get<T>(string key);

        T Get<T>(string key, T defaultValue);

        T Get<T>(Expression<Func<Setting, T>> expr);

        T Get<T>(Expression<Func<Setting, T>> expr, T defaultValue);

        Task SetAsync<T>(Expression<Func<Setting, T>> expr, T value);

        Task RemoveAsync<T>(Expression<Func<Setting, T>> expr);

        T GetTemp<T>(string key);

        T GetTemp<T>(Expression<Func<TempSetting, T>> expr);

        T GetTemp<T>(string key, T defaultValue);

        T GetTemp<T>(Expression<Func<TempSetting, T>> expr, T defaultValue);

        void SetTemp<T>(string key, T value);

        void SetTemp<T>(Expression<Func<TempSetting, T>> expr, T value);

        void RemoveTemp(string key);

        void RemoveTemp<T>(Expression<Func<TempSetting, T>> expr);
    }
}
