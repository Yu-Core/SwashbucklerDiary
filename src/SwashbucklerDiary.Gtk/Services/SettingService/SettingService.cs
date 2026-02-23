using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Gtk.Services
{
    public class SettingService : Rcl.Services.SettingService, ISettingService
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
