using Masa.Blazor;
using SwashbucklerDiary.Rcl.Essentials;
using System.Collections.Concurrent;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Services
{
    public abstract class SettingService : ISettingService
    {
        protected static readonly ConcurrentDictionary<string, object> _defalutSettings = new(new Setting()
                .ToParameters()
                .Where(kv => kv.Value != null)
                .ToDictionary(kv => kv.Key, kv => kv.Value!));

        protected static readonly ConcurrentDictionary<string, object> _defalutTempSettings = new(new Setting()
                .ToParameters()
                .Where(kv => kv.Value != null)
                .ToDictionary(kv => kv.Key, kv => kv.Value!));

        protected IPreferences _preferences;
        protected ConcurrentDictionary<string, object> _tempSettings = [];

        public event Action? SettingsChanged;

        public SettingService(IPreferences preferences)
        {
            _preferences = preferences;
        }

        public virtual Task<bool> ContainsKey(string key)
            => _preferences.ContainsKey(key);

        public virtual Task<T> GetAsync<T>(string key, T defaultValue)
            => _preferences.GetAsync<T>(key, defaultValue);

        public virtual Task SetAsync<T>(string key, T value)
            => _preferences.SetAsync<T>(key, value);

        public virtual Task RemoveAsync(string key)
            => _preferences.RemoveAsync(key);

        public virtual Task RemoveAsync(IEnumerable<string> keys)
            => _preferences.RemoveAsync(keys);

        public virtual Task ClearAsync()
            => _preferences.ClearAsync();

        public T Get<T>(Expression<Func<Setting, T>> expr)
        {
            if (expr.Body is MemberExpression memberExpr)
            {
                return Get<T>(memberExpr.Member.Name);
            }

            throw new ArgumentException("Expression does not contain a member access.");
        }

        public T Get<T>(Expression<Func<Setting, T>> expr, T defaultValue)
        {
            var key = GetSettingKey(expr);
            return Get(key, defaultValue);
        }

        public T Get<T>(string key)
        {
            if (_defalutSettings.TryGetValue(key, out var defaultValue))
            {
                return Get(key, (T)defaultValue);
            }

            return default!;
        }

        public abstract T Get<T>(string key, T defaultValue);

        public Task RemoveAsync<T>(Expression<Func<Setting, T>> expr)
        {
            var key = GetSettingKey(expr);
            return RemoveAsync(key);
        }

        public Task SetAsync<T>(Expression<Func<Setting, T>> expr, T value)
        {
            var key = GetSettingKey(expr);
            return SetAsync(key, value);
        }

        public Setting SaveSettingsToObject(Func<string, bool>? func = null)
        {
            var obj = new Setting();
            var properties = obj.GetType().GetProperties();

            var getMethod = this.GetType()?.GetMethod(nameof(Get), [typeof(string)])
                         ?? throw new InvalidOperationException($"{nameof(Get)} does not exist");

            if (func is not null)
            {
                properties = properties.Where(it => func.Invoke(it.Name)).ToArray();
            }

            foreach (var property in properties)
            {
                if (property.CanWrite)
                {
                    var getGenericMethod = getMethod.MakeGenericMethod(property.PropertyType);
                    var value = getGenericMethod.Invoke(this, [property.Name]);
                    property.SetValue(obj, value);
                }
            }

            return obj;
        }

        public async Task SetSettingsFromObjectAsync(Setting obj, Func<string, bool>? func = null)
        {
            var properties = obj.GetType().GetProperties();
            var setAsyncMethod = this.GetType()?.GetMethod(nameof(SetAsync))
                         ?? throw new InvalidOperationException($"{nameof(SetAsync)} does not exist");
            var getMethod = this.GetType()?.GetMethod(nameof(Get), [typeof(string)])
                         ?? throw new InvalidOperationException($"{nameof(Get)} does not exist");

            if (func is not null)
            {
                properties = properties.Where(it => func.Invoke(it.Name)).ToArray();
            }

            var removeKeys = new List<string>();
            foreach (var property in properties)
            {
                if (property.CanRead && _defalutSettings.TryGetValue(property.Name, out var defaultValue))
                {
                    var value = property.GetValue(obj);
                    var getGenericMethod = getMethod.MakeGenericMethod(property.PropertyType);
                    var currentValue = getGenericMethod.Invoke(this, [property.Name]);

                    if (Object.Equals(value, currentValue))
                    {
                        continue;
                    }

                    if (Object.Equals(value, defaultValue))
                    {
                        removeKeys.Add(property.Name);
                        continue;
                    }

                    var setAsyncGenericMethod = setAsyncMethod.MakeGenericMethod(property.PropertyType);
                    await (Task)setAsyncGenericMethod.Invoke(this, [property.Name, value])!;
                }
            }

            await RemoveAsync(removeKeys);

            SettingsChanged?.Invoke();
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

        public T GetTemp<T>(string key)
        {
            if (_tempSettings.TryGetValue(key, out var value))
            {
                return (T)value;
            }

            if (_defalutTempSettings.TryGetValue(key, out var defaulValue))
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

        public void RemoveTemp<T>(Expression<Func<TempSetting, T>> expr)
        {
            var key = GetTempSettingKey(expr);
            RemoveTemp(key);
        }

        public void RemoveTemp(string key)
        {
            _tempSettings.TryRemove(key, out _);
        }

        public void SetTemp<T>(string key, T value)
        {
            if (value is not null)
            {
                _tempSettings[key] = value;
            }
        }

        public void SetTemp<T>(Expression<Func<TempSetting, T>> expr, T value)
        {
            var key = GetTempSettingKey(expr);
            SetTemp<T>(key, value);
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
    }
}
