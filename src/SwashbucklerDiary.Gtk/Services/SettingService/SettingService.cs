using SwashbucklerDiary.Gtk.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Gtk.Services
{
    public class SettingService : Essentials.Preferences, ISettingService
    {
        public Dictionary<string, object> DefalutSettings { get; set; } = [];
        public Dictionary<string, object> TempSettings { get; set; } = [];
        public Action? SettingsChanged { get; set; }

        public SettingService(UnpackagedPreferencesImplementation preferences) : base(preferences)
        {
        }

        public T Get<T>(string key)
        {
            if (DefalutSettings.TryGetValue(key, out var defaultValue))
            {
                return Get(key, (T)defaultValue);
            }

            return default!;
        }

        public T Get<T>(string key, T defaultValue)
        {
            return _preferences.Get(key, defaultValue);
        }
    }
}
