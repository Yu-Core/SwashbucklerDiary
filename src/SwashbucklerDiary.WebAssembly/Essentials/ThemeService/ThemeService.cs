using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class ThemeService : IThemeService
    {
        private Theme? _theme;

        private readonly SystemThemeJSModule _systemThemeJSModule;

        public event Func<Theme, Task>? OnChanged;

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

        public async Task SetThemeAsync(Theme theme)
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

            await InternalNotifyStateChanged();
        }

        private async Task HandleAppThemeChanged(Theme theme)
        {
            await InternalNotifyStateChanged();
        }

        private async Task InternalNotifyStateChanged()
        {
            if (OnChanged is not null)
            {
                await OnChanged.Invoke(RealTheme);
            }
        }
    }
}
