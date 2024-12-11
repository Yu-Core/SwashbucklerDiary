using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Activity;
using SwashbucklerDiary.Maui.Essentials;
using SwashbucklerDiary.Shared;
using Intent = Android.Content.Intent;

namespace SwashbucklerDiary.Maui
{
#nullable disable
#pragma warning disable CA1416
    [IntentFilter([Intent.ActionView],
        Categories = [Intent.CategoryDefault, Intent.CategoryBrowsable],
        DataSchemes = [SchemeConstants.SwashbucklerDiary, SchemeConstants.XiaKeRiJi])]
    [IntentFilter([Intent.ActionSend],
        Categories = [Intent.CategoryDefault],
        DataMimeType = "text/plain")]
    [IntentFilter([Intent.ActionSend, Intent.ActionSendMultiple],
        Categories = [Intent.CategoryDefault],
        DataMimeTypes = ["image/*", "audio/*", "video/*"])]
    [IntentFilter([Platform.Intent.ActionAppAction],
        Categories = [Intent.CategoryDefault])]
    [Activity(Label = "@string/app_name",
        Theme = "@style/Maui.SplashTheme",
        MainLauncher = true,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density | ConfigChanges.KeyboardHidden,
        LaunchMode = LaunchMode.SingleTask)]
    public class MainActivity : MauiAppCompatActivity
    {
        public override bool DispatchKeyEvent(KeyEvent e)
            => NavigationButtonHandler.OnBackButtonPressed(e);

        protected override void OnCreate(Bundle savedInstanceState)
        {
            EdgeToEdge.Enable(this);
            if (OperatingSystem.IsAndroidVersionAtLeast((int)BuildVersionCodes.Q))
            {
                if (!OperatingSystem.IsAndroidVersionAtLeast(35))
                {
                    Window.StatusBarContrastEnforced = false;
                }

                Window.NavigationBarContrastEnforced = false;
            }

            base.OnCreate(savedInstanceState);
            LaunchActivation.OnLaunched(this.Intent);
            SoftKeyboardAdjustResize.AssistActivity(this);
            SoftKeyboardAdjustResizeHelper.InitBackgroundColor(this);
            Platform.OnNewIntent(this.Intent);
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            LaunchActivation.OnActivated(intent);
            Platform.OnNewIntent(intent);
        }
    }
}
