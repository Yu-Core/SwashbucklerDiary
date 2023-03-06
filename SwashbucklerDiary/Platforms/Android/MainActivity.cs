using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Platforms.Android;

namespace SwashbucklerDiary
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {
#pragma warning disable CS8765 // 参数类型的为 Null 性与重写成员不匹配(可能是由于为 Null 性特性)。
        public override bool DispatchKeyEvent(KeyEvent e)
        {
            if (e.KeyCode == Keycode.Back)
            {
                if (e.Action == KeyEventActions.Down)
                {
                    var service = MauiApplication.Current.Services.GetRequiredService<INavigateService>();
                    service!.OnBackButtonPressed();
                }
                return true;
            }
            return base.DispatchKeyEvent(e);
        }
#pragma warning restore CS8765 // 参数类型的为 Null 性与重写成员不匹配(可能是由于为 Null 性特性)。
        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
#pragma warning disable CA1416 // 验证平台兼容性
                Window!.InsetsController?.SetSystemBarsAppearance((int)WindowInsetsControllerAppearance.LightStatusBars, (int)WindowInsetsControllerAppearance.LightStatusBars);

            }
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M && Build.VERSION.SdkInt < BuildVersionCodes.R)
            {
#pragma warning disable CA1422 // 验证平台兼容性
                Window!.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
#pragma warning restore CA1422 // 验证平台兼容性
            }
#pragma warning restore CA1416 // 验证平台兼容性
            GlobalLayoutUtil.AssistActivity(this);
        }
    }
}
