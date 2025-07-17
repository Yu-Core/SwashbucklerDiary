using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Layout;
using SwashbucklerDiary.Shared;
using SwashbucklerDiary.WebAssembly.Essentials;
using SwashbucklerDiary.WebAssembly.Extensions;

namespace SwashbucklerDiary.WebAssembly.Layout
{
    public partial class MainLayout : MainLayoutBase
    {
        private string? themeColor;

        [Inject]
        private SystemThemeJSModule SystemThemeJSModule { get; set; } = default!;

        [Inject]
        private IAppLifecycle AppLifecycle { get; set; } = default!;

        [Inject]
        private IVersionTracking VersionTracking { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await InitSettingsAsync();
            await InitVersionUpdateAsync();
        }

        protected override void OnDispose()
        {
            ThemeService.OnChanged -= ThemeChanged;
            base.OnDispose();
        }

        protected override async Task InitSettingsAsync()
        {
            await base.InitSettingsAsync();
            await Task.WhenAll(
                InitThemeAsync(),
                InitLanguageAsync(),
                ((Essentials.AppLifecycle)AppLifecycle).InitializedAsync());
        }

        protected override ActivationArguments CreateAppLockActivationArguments()
        {
            return new ActivationArguments()
            {
                Kind = AppActivationKind.Scheme,
                Data = NavigationManager.Uri
            };
        }

        protected override void HandleSchemeActivation(ActivationArguments args, bool replace)
        {
            string? uriString = args?.Data as string;

            if (NavigateController.CheckUrlScheme(NavigationManager, uriString))
            {
                To(uriString, replace: replace);
            }
        }

        private async void ThemeChanged(Theme theme)
        {
            MasaBlazor.SetTheme(theme == Theme.Dark);
            themeColor = theme == Theme.Dark ? ThemeColor.DarkSurface : ThemeColor.LightSurface;
            await InvokeAsync(StateHasChanged);
        }

        private async Task InitThemeAsync()
        {
            ThemeService.OnChanged += ThemeChanged;
            await SystemThemeJSModule.InitializedAsync();
            var theme = SettingService.Get(s => s.Theme);
            ThemeService.SetTheme(theme);
        }

        private Task InitLanguageAsync()
        {
            var language = SettingService.Get(s => s.Language);
            I18n.SetCulture(new(language));
            return Task.CompletedTask;
        }

        private async Task InitVersionUpdateAsync()
        {
            await ((VersionTracking)VersionTracking).Track();
            await VersionUpdataManager.HandleVersionUpdate();
        }

        private async Task ForceRefresh()
        {
            await JSRuntime.InvokeVoidAsync("forceRefresh");
        }

        private async Task RefreshToSkipWaiting()
        {
            await JSRuntime.InvokeVoidAsync("swSkipWaiting");
        }
    }
}
