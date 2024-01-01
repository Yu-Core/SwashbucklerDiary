using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Maui.Essentials
{
    public class Preferences : Rcl.Essentials.Preferences
    {
        public Preferences(IStaticWebAssets staticWebAssets) : base(staticWebAssets)
        {
        }

        public override Task<bool> ContainsKey(string key)
        {
            var result = Microsoft.Maui.Storage.Preferences.Default.ContainsKey(key);
            return Task.FromResult(result);
        }

        public override Task<T> Get<T>(string key, T defaultValue)
        {
            var result = Microsoft.Maui.Storage.Preferences.Default.Get(key, defaultValue);
            return Task.FromResult(result);
        }


        public override Task Remove(string key)
        {
            Microsoft.Maui.Storage.Preferences.Default.Remove(key);
            return Task.CompletedTask;
        }

        public override Task Set<T>(string key, T value)
        {
            Microsoft.Maui.Storage.Preferences.Default.Set(key, value);
            return Task.CompletedTask;
        }
    }
}
