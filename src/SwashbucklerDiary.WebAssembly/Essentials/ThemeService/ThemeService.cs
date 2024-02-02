using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class ThemeService : IThemeService
    {
        private Theme? _theme;

        private readonly SystemThemeJSModule _systemThemeJSModule;

        public event Action<Theme>? OnChanged;

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

        public Task SetThemeAsync(Theme theme)
        {
            if (_theme == theme)
            {
                return Task.CompletedTask;
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
            return Task.CompletedTask;
        }

        private Task HandleAppThemeChanged(Theme theme)
        {
            InternalNotifyStateChanged();
            return Task.CompletedTask;
        }

        private void InternalNotifyStateChanged()
        {
            OnChanged?.Invoke(RealTheme);
        }
    }
}
