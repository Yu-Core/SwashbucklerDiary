using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using NoDecentDiary.IServices;

namespace NoDecentDiary
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
    }
}
#pragma warning restore CS8765 // 参数类型的为 Null 性与重写成员不匹配(可能是由于为 Null 性特性)。