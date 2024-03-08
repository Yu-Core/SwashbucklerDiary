using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Services
{
    public class SettingService : Rcl.Services.SettingService
    {
        public SettingService(Rcl.Essentials.IPreferences preferences,
            IStaticWebAssets staticWebAssets) :
            base(preferences, staticWebAssets)
        {
        }

        public override T Get<T>(Setting setting)
        {
            var key = setting.ToString();
            if (defalutSettings.TryGetValue(key, out var defaultValue))
            {
                return Get(setting, (T)defaultValue);
            }

            return default!;
        }

        public override T Get<T>(Setting setting, T defaultValue)
        {
            string key = setting.ToString();
            return Preferences.Default.Get(key, defaultValue);
        }

        public override Task Set<T>(Setting setting, T value)
        {
            var key = setting.ToString();
            return Set(key, value);
        }
    }
}
