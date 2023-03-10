using Android.OS;
using Android.Views;
using Window = Android.Views.Window;

namespace SwashbucklerDiary.Platforms.Android
{
#pragma warning disable CA1416 // 验证平台兼容性
#pragma warning disable CA1422 // 验证平台兼容性
#nullable disable
    public static class StatusBarSetting
    {
        public static void SetStatusBar(Window window)
        {
            if (Build.VERSION.SdkInt >= BuildVersionCodes.R)
            {
                window.InsetsController.SetSystemBarsAppearance((int)WindowInsetsControllerAppearance.LightStatusBars, (int)WindowInsetsControllerAppearance.LightStatusBars);
            }
            if (Build.VERSION.SdkInt >= BuildVersionCodes.M && Build.VERSION.SdkInt < BuildVersionCodes.R)
            {
                window.DecorView.SystemUiVisibility = (StatusBarVisibility)SystemUiFlags.LightStatusBar;
            }
        }
    }
}
