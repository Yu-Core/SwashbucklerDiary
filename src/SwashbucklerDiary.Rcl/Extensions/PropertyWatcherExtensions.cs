using Masa.Blazor;
using System.Reflection;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static class PropertyWatcherExtensions
    {
        private static readonly MethodInfo? _getOrSetPropertyMethod = typeof(PropertyWatcher).GetMethod("GetOrSetProperty", BindingFlags.NonPublic | BindingFlags.Instance);

        public static ObservableProperty<TValue>? GetOrSetProperty<TValue>(this PropertyWatcher _watcher, TValue? @default, string name, bool disableIListAlwaysNotifying = false,
            bool fromSetter = false)
        {
            if (_getOrSetPropertyMethod == null)
            {
                throw new InvalidOperationException("GetOrSetProperty method not found.");
            }

            MethodInfo genericMethod = _getOrSetPropertyMethod.MakeGenericMethod(typeof(TValue));
            var result = genericMethod.Invoke(_watcher, [@default, name, disableIListAlwaysNotifying, fromSetter]);
            return result as ObservableProperty<TValue>;
        }
    }
}
