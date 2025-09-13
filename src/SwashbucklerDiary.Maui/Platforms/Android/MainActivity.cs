using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using AndroidX.Activity;
using SwashbucklerDiary.Maui.Essentials;
using SwashbucklerDiary.Rcl.Essentials;
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
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density | ConfigChanges.KeyboardHidden | ConfigChanges.Keyboard | ConfigChanges.FontScale,
        LaunchMode = LaunchMode.SingleTask)]
    public class MainActivity : MauiAppCompatActivity
    {
        private SoftKeyboardAdjustResize softKeyboardAdjustResize;

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
            AppActivation.Launch(this.Intent);
            softKeyboardAdjustResize = new SoftKeyboardAdjustResize(this);
            SoftKeyboardAdjustResizeHelper.InitBackgroundColor(this);
            Platform.OnNewIntent(this.Intent);

            OnBackPressedDispatcher.AddCallback(new OnBackPressedCallback(true));
        }

        protected override void OnNewIntent(Intent intent)
        {
            base.OnNewIntent(intent);

            AppActivation.Activate(intent);
            Platform.OnNewIntent(intent);
        }

        protected override void OnStop()
        {
            base.OnStop();

            softKeyboardAdjustResize.OnStop();
        }

        class OnBackPressedCallback(bool enabled) : AndroidX.Activity.OnBackPressedCallback(enabled)
        {
            private readonly Lazy<INavigateController> _navigateController = new(() => IPlatformApplication.Current!.Services.GetRequiredService<INavigateController>());
            private INavigateController NavigateController => _navigateController.Value;

            public override void HandleOnBackPressed()
            {
                NavigateController.BackPressed();
            }
        }
    }
}
