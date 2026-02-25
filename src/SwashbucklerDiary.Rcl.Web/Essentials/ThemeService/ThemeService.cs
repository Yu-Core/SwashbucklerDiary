using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Web.Essentials
{
    public class ThemeService : IThemeService
    {
        private Theme? _theme;

        private readonly SystemThemeJSModule _systemThemeJSModule;

        public event Action<Theme>? ThemeChanged;

        public Theme RealTheme => _theme switch
        {
            Theme.System => _systemThemeJSModule.SystemTheme,
            Theme.Dark => Theme.Dark,
            _ => Theme.Light
        };

        public ThemeService(SystemThemeJSModule systemThemeJSModule)
        {
            _systemThemeJSModule = systemThemeJSModule;
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
                _systemThemeJSModule.SystemThemeChanged += HandleAppThemeChanged;
            }
            //取消跟随系统主题改变
            else if (_theme == Theme.System)
            {
                _systemThemeJSModule.SystemThemeChanged -= HandleAppThemeChanged;
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
            ThemeChanged?.Invoke(RealTheme);
        }
    }
}
