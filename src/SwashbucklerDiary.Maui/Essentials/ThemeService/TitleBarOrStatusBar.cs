using SwashbucklerDiary.Shared;
using SwashbucklerDiary.Rcl;

#if WINDOWS || MACCATALYST
using MauiBlazorToolkit;
using MauiBlazorToolkit.Platform;
#elif ANDROID || IOS
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Platform;
#endif

namespace SwashbucklerDiary.Maui.Essentials
{
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
            TitleBar.SetColor(backgroundColor);
            TitleBarStyle titleBarStyle = dark ? TitleBarStyle.LightContent : TitleBarStyle.DarkContent;
            TitleBar.SetStyle(titleBarStyle);
#elif ANDROID || IOS14_2_OR_GREATER
            StatusBar.SetColor(backgroundColor);
            StatusBarStyle statusBarStyle = dark ? StatusBarStyle.LightContent : StatusBarStyle.DarkContent;
            StatusBar.SetStyle(statusBarStyle);
#endif

#if IOS
            MainPage.SetIOSGapColor(backgroundColor);
#endif
        }
    }
}
