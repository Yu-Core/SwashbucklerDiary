namespace SwashbucklerDiary.Maui.Services
{
    public class SettingService : Rcl.Services.SettingService
    {
        public SettingService(Rcl.Essentials.IPreferences preferences) :
            base(preferences)
        {
        }

        public override T Get<T>(string key)
        {
            if (_defalutSettings.TryGetValue(key, out var defaultValue))
            {
                return Get(key, (T)defaultValue);
            }

            return default!;
        }

        public override T Get<T>(string key, T defaultValue)
        {
            return Preferences.Default.Get(key, defaultValue);
        }
    }
}
