using Android.App;
using SwashbucklerDiary.Rcl;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Maui
{
    public static class SoftKeyboardAdjustResizeHelper
    {
        static readonly Android.Graphics.Color lightColor = Android.Graphics.Color.ParseColor(ThemeColor.LightSurface);

        static readonly Android.Graphics.Color darkColor = Android.Graphics.Color.ParseColor(ThemeColor.DarkSurface);

        public static void InitBackgroundColor(Activity activity)
        {
            var themeService = IPlatformApplication.Current.Services.GetRequiredService<IThemeService>();
            HandleOnThemeChanged(themeService.RealTheme);
            themeService.OnChanged += HandleOnThemeChanged;

            void HandleOnThemeChanged(Shared.Theme theme)
            {
                var color = theme == Shared.Theme.Dark ? darkColor : lightColor;
                SoftKeyboardAdjustResize.SetBackgroundColor(activity, color);
            }
        }
    }
}
