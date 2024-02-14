using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Layout;
using SwashbucklerDiary.Shared;
using SwashbucklerDiary.WebAssembly.Essentials;

namespace SwashbucklerDiary.WebAssembly.Layout
{
    public partial class MainLayout : MainLayoutBase
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        [Inject]
        private SystemThemeJSModule SystemThemeJSModule { get; set; } = default!;

        [Inject]
        private IAppLifecycle AppLifecycle { get; set; } = default!;

        [Inject]
        private IVersionTracking VersionTracking { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await InitVersionUpdateAsync();
            await Task.WhenAll(
                InitThemeAsync(),
                InitLanguageAsync(),
                ((AppLifecycle)AppLifecycle).InitializedAsync());
        }

        protected override void OnDispose()
        {
            ThemeService.OnChanged -= ThemeChanged;
            base.OnDispose();
        }

        private async Task InitThemeAsync()
        {
            ThemeService.OnChanged += ThemeChanged;
            await SystemThemeJSModule.InitializedAsync();
            var themeState = await SettingService.Get<int>(Setting.Theme);
            await ThemeService.SetThemeAsync((Theme)themeState);
        }

        private async Task InitLanguageAsync()
        {
            var language = await SettingService.Get<string>(Setting.Language);
            I18n.SetCulture(language);
        }

        private async Task InitVersionUpdateAsync()
        {
            await ((VersionTracking)VersionTracking).Track(typeof(App).Assembly);
            await VersionManager.UpdateVersion();
        }

        private async Task RefreshPage()
        {
            await RefreshToSkipWaiting();
        }

        private async Task RefreshToSkipWaiting()
        {
            await JSRuntime.InvokeVoidAsync("swSkipWaiting");
        }
    }
}
