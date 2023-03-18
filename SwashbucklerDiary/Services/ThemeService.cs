using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Services
{
    public class ThemeService : IThemeService
    {
        private ThemeState _themeState = ThemeState.Light;
        public ThemeState ThemeState
        {
            get => GetThemeState();
            set => SetThemeState(value);
        }
        public bool System => _themeState == ThemeState.System;
        public bool Light => _themeState == ThemeState.Light;
        public bool Dark => _themeState == ThemeState.Dark;

        public event Action<ThemeState>? OnChanged;

        private ThemeState GetThemeState()
        {
            if (_themeState == ThemeState.System)
            {
                return Application.Current!.RequestedTheme == AppTheme.Dark ? ThemeState.Dark : ThemeState.Light;   
            }
            
            return _themeState;
        }

        /// <summary>
        /// 系统主题切换
        /// </summary>
        private void SetThemeState(ThemeState themeState)
        {
            _themeState = themeState;
            if (themeState == ThemeState.System)
            {
                Application.Current!.RequestedThemeChanged += HandlerAppThemeChanged;
            }
            else
            {
                Application.Current!.RequestedThemeChanged -= HandlerAppThemeChanged;
            }
            OnChanged?.Invoke(ThemeState);
        }

        private void HandlerAppThemeChanged(object? sender, AppThemeChangedEventArgs e)
        {
            OnChanged?.Invoke(ThemeState);
        }
    }
}
