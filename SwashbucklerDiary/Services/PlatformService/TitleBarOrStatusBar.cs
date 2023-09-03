using SwashbucklerDiary.Models;

#if WINDOWS || MACCATALYST
using MauiBlazorToolkit;
using MauiBlazorToolkit.Platform;
#elif ANDROID || IOS
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Platform;
#endif

namespace SwashbucklerDiary.Services
{
#pragma warning disable CA1416
    public partial class PlatformService
    {
        public Color LightColor { get; set; } = Color.FromRgb(247, 248, 249);
        public Color DarkColor { get; set; } = Color.FromRgb(18, 18, 18);

        public void SetTitleBarOrStatusBar(ThemeState themeState)
        {
            Color backgroundColor = themeState == ThemeState.Dark ? DarkColor : LightColor;
            SetTitleBarOrStatusBar(themeState, backgroundColor);
        }

        public void SetTitleBarOrStatusBar(ThemeState themeState, Color backgroundColor)
        {
            if (themeState == ThemeState.System)
            {
                return;
            }

            bool Dark = themeState == ThemeState.Dark;
#if WINDOWS || MACCATALYST
            TitleBar.SetColor(backgroundColor);
            TitleBarStyle titleBarStyle = Dark ? TitleBarStyle.LightContent : TitleBarStyle.DarkContent;
            TitleBar.SetStyle(titleBarStyle);
#elif ANDROID || IOS14_2_OR_GREATER
            StatusBar.SetColor(backgroundColor);
            StatusBarStyle statusBarStyle = Dark ? StatusBarStyle.LightContent : StatusBarStyle.DarkContent;
            StatusBar.SetStyle(statusBarStyle);
#endif
        }
    }
}
