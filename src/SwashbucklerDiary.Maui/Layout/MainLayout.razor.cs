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
            await InitThemeAsync();
        }

        protected override void OnDispose()
        {
            ThemeService.OnChanged -= ThemeChanged;
            base.OnDispose();
        }

        private async Task InitThemeAsync()
        {
            ThemeService.OnChanged += ThemeChanged;
            var themeState = await Preferences.Get<int>(Setting.Theme);
            await ThemeService.SetThemeAsync((Theme)themeState);
        }
    }
}
