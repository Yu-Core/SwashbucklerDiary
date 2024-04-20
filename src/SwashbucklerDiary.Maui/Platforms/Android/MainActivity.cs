using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Core.View;

namespace SwashbucklerDiary.Maui
{
#nullable disable
    [Activity(Label = "@string/app_name", Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density | ConfigChanges.KeyboardHidden)]
    public class MainActivity : MauiAppCompatActivity
    {
        public override bool DispatchKeyEvent(KeyEvent e)
            => NavigationButtonHandler.OnBackButtonPressed(e) || base.DispatchKeyEvent(e);

        protected override void OnCreate(Bundle savedInstanceState)
        {
            WindowCompat.SetDecorFitsSystemWindows(Window, false);
            Window.SetStatusBarColor(Android.Graphics.Color.Transparent);
            Window.SetNavigationBarColor(Android.Graphics.Color.Transparent);

            base.OnCreate(savedInstanceState);
            SoftKeyboardAdjustResize.Initialize();
        }
    }
}
