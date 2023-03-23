using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using SwashbucklerDiary.Platforms.Android;

namespace SwashbucklerDiary
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
#nullable disable
        public override bool DispatchKeyEvent(KeyEvent e)
        {
            var flag = BackButtonPressed.OnBackButtonPressed(e);
            if (flag)
            {
                return true;
            }

            return base.DispatchKeyEvent(e);
        }
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            GlobalLayoutUtil.AssistActivity(this);
        }
    }
}
