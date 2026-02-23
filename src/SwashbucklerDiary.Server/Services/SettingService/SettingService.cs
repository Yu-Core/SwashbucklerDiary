using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Server.Services
{
    public class SettingService : Rcl.Services.SettingService
    {
        public SettingService(IPreferences preferences) : base(preferences)
        {
        }

        public override T Get<T>(string key, T defaultValue)
        {
            return Microsoft.Maui.Storage.Preferences.Default.Get(key, defaultValue);
        }
    }
}
