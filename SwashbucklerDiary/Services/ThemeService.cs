using CommunityToolkit.Maui.Core;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

#if WINDOWS || MACCATALYST
using MauiBlazorToolkit;
using MauiBlazorToolkit.Platform;
#endif

namespace SwashbucklerDiary.Services
{
    public class ThemeService : IThemeService
    {
        private ThemeState? _themeState;
        public ThemeState ThemeState
        {
            get => GetThemeState();
        }
        public bool Light => ThemeState == ThemeState.Light;
        public bool Dark => ThemeState == ThemeState.Dark;

        public event Action<ThemeState>? OnChanged;

        private ThemeState GetThemeState()
        {
            if (_themeState == ThemeState.System)
            {
                return Application.Current!.RequestedTheme == AppTheme.Dark ? ThemeState.Dark : ThemeState.Light;   
            }
            
            return _themeState ?? ThemeState.Light;
        }

        /// <summary>
        /// 系统主题切换
        /// </summary>
        public void SetThemeState(ThemeState value)
        {
            if (_themeState == value)
            {
                return;
            }

            _themeState = value;
            if (value == ThemeState.System)
            {
                Application.Current!.RequestedThemeChanged += HandlerAppThemeChanged;
            }
            else
            {
                Application.Current!.RequestedThemeChanged -= HandlerAppThemeChanged;
            }

            InternalNotifyStateChanged(ThemeState);
        }

        private void HandlerAppThemeChanged(object? sender, AppThemeChangedEventArgs e)
        {
            InternalNotifyStateChanged(ThemeState);
        }

        private void InternalNotifyStateChanged(ThemeState value)
        {
            SetStatusBar(value);
            OnChanged?.Invoke(value);
        }

        private static readonly Color statusBarColorLight = Color.FromRgb(247, 248, 249);
        private static readonly Color statusBarColorDark = Color.FromRgb(18, 18, 18);
#pragma warning disable CA1416
        private static void SetStatusBar(ThemeState themeState)
        {
            var Dark = themeState == ThemeState.Dark;
            Color backgroundColor = Dark ? statusBarColorDark : statusBarColorLight;
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
