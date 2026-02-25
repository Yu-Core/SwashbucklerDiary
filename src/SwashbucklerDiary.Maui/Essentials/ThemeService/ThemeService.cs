using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Essentials
{
    public class ThemeService : IThemeService
    {
        private Theme? _theme;

        public event Action<Theme>? ThemeChanged;

        public Theme RealTheme => _theme switch
        {
            Theme.System => Application.Current!.RequestedTheme == AppTheme.Dark ? Theme.Dark : Theme.Light,
            Theme.Dark => Theme.Dark,
            _ => Theme.Light
        };

        public void SetTheme(Theme theme)
        {
            if (_theme == theme)
            {
                return;
            }

            //跟随系统主题改变
            if (theme == Theme.System)
            {
                Application.Current!.RequestedThemeChanged += HandleAppThemeChanged;
            }
            //取消跟随系统主题改变
            else if (_theme == Theme.System)
            {
                Application.Current!.RequestedThemeChanged -= HandleAppThemeChanged;
            }

            _theme = theme;

            InternalNotifyStateChanged();
        }

        private void HandleAppThemeChanged(object? sender, AppThemeChangedEventArgs e)
        {
            InternalNotifyStateChanged();
        }

        private void InternalNotifyStateChanged()
        {
            ThemeChanged?.Invoke(RealTheme);
        }
    }
}
