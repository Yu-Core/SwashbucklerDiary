using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Maui.Services
{
    public class SettingService : Rcl.Services.SettingService, ISettingService
    {
        public SettingService(Rcl.Essentials.IPreferences preferences) : base(preferences)
        {
        }

        public override T Get<T>(string key, T defaultValue)
        {
            return Microsoft.Maui.Storage.Preferences.Default.Get(key, defaultValue);
        }
    }
}
