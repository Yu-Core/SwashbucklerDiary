using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Gtk.Essentials
{
    public class ThemeService : IThemeService
    {
        private Theme? _theme;

        private readonly GtkSystemThemeManager _gtkSystemThemeManager;

        public event Action<Theme>? OnChanged;

        public Theme RealTheme => _theme switch
        {
            Theme.System => _gtkSystemThemeManager.SystemTheme,
            Theme.Dark => Theme.Dark,
            _ => Theme.Light
        };

        public ThemeService(GtkSystemThemeManager gtkSystemThemeManager)
        {
            _gtkSystemThemeManager = gtkSystemThemeManager;
        }

        public void SetTheme(Theme theme)
        {
            if (_theme == theme)
            {
                return;
            }

            //跟随系统主题改变
            if (theme == Theme.System)
            {
                _gtkSystemThemeManager.SystemThemeChanged += HandleAppThemeChanged;
            }
            //取消跟随系统主题改变
            else if (_theme == Theme.System)
            {
                _gtkSystemThemeManager.SystemThemeChanged -= HandleAppThemeChanged;
            }

            _theme = theme;

            InternalNotifyStateChanged();
        }

        private void HandleAppThemeChanged(Theme theme)
        {
            InternalNotifyStateChanged();
        }

        private void InternalNotifyStateChanged()
        {
            OnChanged?.Invoke(RealTheme);
        }
    }
}
