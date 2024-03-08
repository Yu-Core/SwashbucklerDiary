using SwashbucklerDiary.Rcl.Layout;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui.Layout
{
    public partial class MainLayout : MainLayoutBase
    {
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await VersionManager.UpdateVersion();
            await InitSettingsAsync();
        }

        protected override void OnDispose()
        {
            ThemeService.OnChanged -= ThemeChanged;
            base.OnDispose();
        }

        protected override async Task InitSettingsAsync()
        {
            await base.InitSettingsAsync();
            await InitThemeAsync();
        }

        private async Task InitThemeAsync()
        {
            ThemeService.OnChanged += ThemeChanged;
            var themeState = SettingService.Get<int>(Setting.Theme);
            await ThemeService.SetThemeAsync((Theme)themeState);
        }
    }
}
