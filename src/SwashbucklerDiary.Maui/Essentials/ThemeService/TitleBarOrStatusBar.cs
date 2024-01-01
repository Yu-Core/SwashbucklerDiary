using SwashbucklerDiary.Shared;

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
        public static Color LightColor { get; set; } = Color.FromArgb("#f7f8f9");

        public static Color DarkColor { get; set; } = Color.FromArgb("#121212");

        public static void SetTitleBarOrStatusBar(Theme theme)
        {
            Color backgroundColor = theme == Theme.Dark ? DarkColor : LightColor;
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
        }
    }
}
