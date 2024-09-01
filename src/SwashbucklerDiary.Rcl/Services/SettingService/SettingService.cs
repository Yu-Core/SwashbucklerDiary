using Masa.Blazor;
using SwashbucklerDiary.Rcl.Essentials;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Services
{
    public abstract class SettingService : ISettingService
    {
        private readonly IPreferences _preferences;

        protected Dictionary<string, object> _defalutSettings = [];

        protected Dictionary<string, object> _tempSettings = [];

        public SettingService(IPreferences preferences)
        {
            _preferences = preferences;
        }

        public virtual Task InitializeAsync()
        {
            var defaultSettings = new Setting().ToParameters().ToDictionary();
            var defaultTempSettings = new TempSetting().ToParameters().ToDictionary();

            AddDefaultSettings(defaultSettings);
            AddDefaultSettings(defaultTempSettings);

            return Task.CompletedTask;
        }

        private void AddDefaultSettings(Dictionary<string, object?> dictionary)
        {
            foreach (var item in dictionary)
            {
                if (item.Value is not null)
                {
                    _defalutSettings.Add(item.Key, item.Value);
                }
            }
        }

        public abstract T Get<T>(string key);

        public abstract T Get<T>(string key, T defaultValue);

        public virtual Task<bool> ContainsKey(string key)
            => _preferences.ContainsKey(key);

        public virtual Task<T> GetAsync<T>(string key, T defaultValue)
            => _preferences.GetAsync(key, defaultValue);

        public virtual Task RemoveAsync(string key)
            => _preferences.RemoveAsync(key);

        public virtual Task RemoveAsync(IEnumerable<string> keys)
            => _preferences.RemoveAsync(keys);

        public virtual Task SetAsync<T>(string key, T value)
            => _preferences.SetAsync(key, value);

        public virtual Task ClearAsync()
            => _preferences.ClearAsync();

        public T GetTemp<T>(string key)
        {
            if (_tempSettings.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            if (_defalutSettings.TryGetValue(key, out var defaulValue))
            {
                return (T)defaulValue;
            }

            return default!;
        }

        public T GetTemp<T>(string key, T defaultValue)
        {
            if (_tempSettings.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            return defaultValue;
        }

        public void SetTemp<T>(string key, T value)
        {
            _tempSettings[key] = value;
        }

        public void RemoveTemp(string key)
        {
            _tempSettings.Remove(key);
        }

        public T Get<T>(Expression<Func<Setting, T>> expr)
        {
            if (expr.Body is MemberExpression memberExpr)
            {
                return Get<T>(memberExpr.Member.Name);
            }

            throw new ArgumentException("Expression does not contain a member access.");
        }

        public Task SetAsync<T>(Expression<Func<Setting, T>> expr, T value)
        {
            var key = GetSettingKey(expr);
            return SetAsync(key, value);
        }

        private static string GetSettingKey<T>(Expression<Func<Setting, T>> expr) => GetExpressionMemberName(expr);

        private static string GetTempSettingKey<T>(Expression<Func<TempSetting, T>> expr) => GetExpressionMemberName(expr);

        private static string GetExpressionMemberName<T, TResult>(Expression<Func<T, TResult>> expr)
        {
            if (expr.Body is MemberExpression memberExpr)
            {
                return memberExpr.Member.Name;
            }

            throw new ArgumentException("Expression does not contain a member access.");
        }

        public T GetTemp<T>(Expression<Func<TempSetting, T>> expr)
        {
            var key = GetTempSettingKey(expr);
            return GetTemp<T>(key);
        }

        public T GetTemp<T>(Expression<Func<TempSetting, T>> expr, T defaultValue)
        {
            var key = GetTempSettingKey(expr);
            return GetTemp<T>(key, defaultValue);
        }

        public void SetTemp<T>(Expression<Func<TempSetting, T>> expr, T value)
        {
            var key = GetTempSettingKey(expr);
            SetTemp<T>(key, value);
        }

        public void RemoveTemp<T>(Expression<Func<TempSetting, T>> expr)
        {
            var key = GetTempSettingKey(expr);
            RemoveTemp(key);
        }

        public T Get<T>(Expression<Func<Setting, T>> expr, T defaultValue)
        {
            var key = GetSettingKey(expr);
            return GetTemp(key, defaultValue);
        }

        public Task RemoveAsync<T>(Expression<Func<Setting, T>> expr)
        {
            var key = GetSettingKey(expr);
            return RemoveAsync(key);
        }
    }
}
