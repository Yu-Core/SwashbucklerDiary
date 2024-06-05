using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using SwashbucklerDiary.Maui.Essentials;
using Intent = Android.Content.Intent;

namespace SwashbucklerDiary.Maui
{
#nullable disable
#pragma warning disable CA1416
    [IntentFilter([Intent.ActionView],
        Categories = [Intent.CategoryDefault, Intent.CategoryBrowsable],
        DataScheme = "swashbucklerdiary")]
    [IntentFilter([Intent.ActionSend],
        Categories = [Intent.CategoryDefault],
        DataMimeType = "text/plain")]
    //Vidoe files may be relatively large and time-consuming to copy, so they are temporarily disabled
    [IntentFilter([Intent.ActionSend, Intent.ActionSendMultiple],
        Categories = [Intent.CategoryDefault],
        DataMimeTypes = ["image/*", "audio/*", /*"video/*"*/])]
    [Activity(Label = "@string/app_name",
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density | ConfigChanges.KeyboardHidden,
        LaunchMode = LaunchMode.SingleTask)]
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
            LaunchActivation.HandleOnLaunched(this.Intent);
            SoftKeyboardAdjustResize.Initialize();
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            LaunchActivation.OnApplicationActivated(intent);
        }
    }
}
