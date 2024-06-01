using Java.Lang;
using System.Collections;

namespace SwashbucklerDiary.Maui.Extensions
{
    public static class IntentExtensions
    {
        public static T? GetParcelableExtra<T>(this Android.Content.Intent intent, string? name) where T : Java.Lang.Object
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                return (T?)intent.GetParcelableExtra(name, Class.FromType(typeof(T)));
            }

            return (T?)intent.GetParcelableExtra(name);
        }

        public static List<T>? GetParcelableArrayListExtra<T>(this Android.Content.Intent intent, string? name) where T : Java.Lang.Object
        {
            IList? data;
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                data = intent.GetParcelableArrayListExtra(name, Class.FromType(typeof(T)));
            }
            else
            {
                data = intent.GetParcelableArrayListExtra(name);
            }

            List<T> list = [];
            if (data is not null)
            {
                foreach (var item in data)
                {
                    if (item is T t)
                    {
                        list.Add(t);
                    }
                }
            }

            return list;
        }
    }
}
