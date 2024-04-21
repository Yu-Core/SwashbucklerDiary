using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;

namespace SwashbucklerDiary.Maui
{
#nullable disable
#pragma warning disable CA1416
    [Activity(Label = "@string/app_name", Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density | ConfigChanges.KeyboardHidden)]
    public class MainActivity : MauiAppCompatActivity
    {
        public override bool DispatchKeyEvent(KeyEvent e)
            => NavigationButtonHandler.OnBackButtonPressed(e) || base.DispatchKeyEvent(e);

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Google.Android.Material.Internal.EdgeToEdgeUtils.ApplyEdgeToEdge(Window, true);
            if (OperatingSystem.IsAndroidVersionAtLeast((int)BuildVersionCodes.Q))
            {
                Window.StatusBarContrastEnforced = false;
                Window.NavigationBarContrastEnforced = false;
            }

            base.OnCreate(savedInstanceState);
            SoftKeyboardAdjustResize.Initialize();
        }
    }
}
