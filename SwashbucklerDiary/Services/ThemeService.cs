using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

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

            OnChanged?.Invoke(ThemeState);
        }

        private void HandlerAppThemeChanged(object? sender, AppThemeChangedEventArgs e)
        {
            OnChanged?.Invoke(ThemeState);
        }
    }
}
