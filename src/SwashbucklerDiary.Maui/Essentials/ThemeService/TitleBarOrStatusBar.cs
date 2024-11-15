using SwashbucklerDiary.Shared;
using SwashbucklerDiary.Rcl;

#if WINDOWS || MACCATALYST
using MauiBlazorToolkit;
using MauiBlazorToolkit.Platform;
#elif IOS
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Platform;
#endif

#if ANDROID
using Android.Views;
using AndroidX.Core.View;
#endif

namespace SwashbucklerDiary.Maui.Essentials
{
#nullable disable
#pragma warning disable CA1416
    public class TitleBarOrStatusBar
    {
        private static readonly Color lightColor = Color.FromArgb(ThemeColor.LightSurface);

        private static readonly Color darkColor = Color.FromArgb(ThemeColor.DarkSurface);

        public static void SetTitleBarOrStatusBar(Theme theme)
        {
            Color backgroundColor = theme == Theme.Dark ? darkColor : lightColor;
            SetTitleBarOrStatusBar(theme, backgroundColor);
        }

        public static void SetTitleBarOrStatusBar(Theme theme, Color backgroundColor)
        {
            if (theme == Theme.System)
            {
                return;
            }

            bool dark = theme == Theme.Dark;
#if WINDOWS || MACCATALYST
            MauiBlazorToolkit.Platform.TitleBar.SetColor(backgroundColor);
            TitleBarStyle titleBarStyle = dark ? TitleBarStyle.LightContent : TitleBarStyle.DarkContent;
            MauiBlazorToolkit.Platform.TitleBar.SetStyle(titleBarStyle);
#elif ANDROID
            var window = Platform.CurrentActivity.Window;
            var windowController = WindowCompat.GetInsetsController(window, window.DecorView);
            windowController.AppearanceLightStatusBars = !dark;
            windowController.AppearanceLightNavigationBars = !dark;
#elif IOS14_2_OR_GREATER
            StatusBarStyle statusBarStyle = dark ? StatusBarStyle.LightContent : StatusBarStyle.DarkContent;
            StatusBar.SetStyle(statusBarStyle);
#endif
        }
    }
}
