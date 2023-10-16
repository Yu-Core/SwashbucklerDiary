using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Services
{
    public class ThemeService : IThemeService
    {
        public event Action<ThemeState>? OnChanged;

        private readonly IPlatformService PlatformService;

        public ThemeState? ThemeState { get; set; }

        public ThemeState RealThemeState => GetThemeState();

        public bool Light => RealThemeState == Models.ThemeState.Light;

        public bool Dark => RealThemeState == Models.ThemeState.Dark;

        public ThemeService(IPlatformService platformService) 
        {
            PlatformService = platformService;
        }

        private ThemeState GetThemeState()
        {
            if (ThemeState == Models.ThemeState.System)
            {
                return Application.Current!.RequestedTheme == AppTheme.Dark ? Models.ThemeState.Dark : Models.ThemeState.Light;   
            }
            
            return ThemeState ?? Models.ThemeState.Light;
        }

        /// <summary>
        /// 系统主题切换
        /// </summary>
        public void SetThemeState(ThemeState value)
        {
            if (ThemeState == value)
            {
                return;
            }

            ThemeState = value;
            if (value == Models.ThemeState.System)
            {
                Application.Current!.RequestedThemeChanged += HandlerAppThemeChanged;
            }
            else
            {
                Application.Current!.RequestedThemeChanged -= HandlerAppThemeChanged;
            }

            InternalNotifyStateChanged();
        }

        private void HandlerAppThemeChanged(object? sender, AppThemeChangedEventArgs e)
        {
            InternalNotifyStateChanged();
        }

        private void InternalNotifyStateChanged()
        {
            ThemeState value = RealThemeState;
            PlatformService.SetTitleBarOrStatusBar(value);
            OnChanged?.Invoke(value);
        }
    }
}
