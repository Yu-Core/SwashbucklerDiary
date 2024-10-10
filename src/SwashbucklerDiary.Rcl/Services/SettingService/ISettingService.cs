using Masa.Blazor;
using SwashbucklerDiary.Rcl.Essentials;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Services
{
    public interface ISettingService : IPreferences
    {
        protected Dictionary<string, object> DefalutSettings { get; set; }

        protected Dictionary<string, object> TempSettings { get; set; }

        public Task InitializeAsync() => DefalutInitializeAsync(DefalutSettings);

        protected static Task DefalutInitializeAsync(Dictionary<string, object> dictionary)
        {
            var defaultSettings = new Setting().ToParameters().ToDictionary();
            var defaultTempSettings = new TempSetting().ToParameters().ToDictionary();

            AddDefaultSettings(dictionary, defaultSettings);
            AddDefaultSettings(dictionary, defaultTempSettings);

            return Task.CompletedTask;
        }

        private static void AddDefaultSettings(Dictionary<string, object> dictionary, Dictionary<string, object?> dictionary2)
        {
            foreach (var item in dictionary2)
            {
                if (item.Value is not null)
                {
                    dictionary.Add(item.Key, item.Value);
                }
            }
        }

        public T Get<T>(string key);

        public T Get<T>(string key, T defaultValue);

        public T GetTemp<T>(string key)
        {
            if (TempSettings.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            if (DefalutSettings.TryGetValue(key, out var defaulValue))
            {
                return (T)defaulValue;
            }

            return default!;
        }

        public T GetTemp<T>(string key, T defaultValue)
        {
            if (TempSettings.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            return defaultValue;
        }

        public void SetTemp<T>(string key, T value)
        {
            TempSettings[key] = value;
        }

        public void RemoveTemp(string key)
        {
            TempSettings.Remove(key);
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
