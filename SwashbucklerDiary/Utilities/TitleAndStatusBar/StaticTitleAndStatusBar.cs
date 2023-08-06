using SwashbucklerDiary.Models;

#if WINDOWS || MACCATALYST
using MauiBlazorToolkit;
using MauiBlazorToolkit.Platform;
#elif ANDROID || IOS
using CommunityToolkit.Maui.Core;
#endif

namespace SwashbucklerDiary.Utilities
{
    public static class StaticTitleAndStatusBar
    {
        private static readonly Color lightColor = Color.FromRgb(247, 248, 249);
        private static readonly Color darkColor = Color.FromRgb(18, 18, 18);
#pragma warning disable CA1416
        public static void SetTitleAndStatusBar(ThemeState themeState)
        {
            var Dark = themeState == Models.ThemeState.Dark;
            Color backgroundColor = Dark ? darkColor : lightColor;
#if WINDOWS || MACCATALYST
            TitleBar.SetColor(backgroundColor);
            TitleBarStyle titleBarStyle = Dark ? TitleBarStyle.LightContent : TitleBarStyle.DarkContent;
            TitleBar.SetStyle(titleBarStyle);
#elif ANDROID || IOS14_2_OR_GREATER
            CommunityToolkit.Maui.Core.Platform.StatusBar.SetColor(backgroundColor);
            StatusBarStyle statusBarStyle = Dark ? StatusBarStyle.LightContent : StatusBarStyle.DarkContent;
            CommunityToolkit.Maui.Core.Platform.StatusBar.SetStyle(statusBarStyle);
#endif
        }
    }
}
