using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Web.Essentials;
using SwashbucklerDiary.Rcl.Web.Extensions;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Web.Layout
{
    public class MainLayoutBase : Rcl.Layout.MainLayoutBase
    {
        protected string? themeColor;

        [Inject]
        protected SystemThemeJSModule SystemThemeJSModule { get; set; } = default!;

        [Inject]
        private RouteMatcher RouteMatcher { get; set; } = default!;

        protected override void OnDispose()
        {
            ThemeService.ThemeChanged -= ThemeChanged;
            base.OnDispose();
        }

        protected override void HandleSchemeActivation(ActivationArguments args, bool replace)
        {
            string? uriString = args?.Data as string;

            if (uriString is not null && RouteMatcher.CheckUrlScheme(NavigationManager, uriString))
            {
                To(uriString, replace: replace);
            }
        }

        protected override async Task InitConfigAsync()
        {
            await base.InitConfigAsync();
            await InitThemeAsync();
            await InitLanguageAsync();
            await ((Rcl.Web.Essentials.AppLifecycle)AppLifecycle).InitializedAsync();
        }

        protected async Task ForceRefresh()
        {
            await JSRuntime.InvokeVoidAsync("forceRefresh");
        }

        private async void ThemeChanged(Theme theme)
        {
            MasaBlazor.SetTheme(theme == Theme.Dark);

            await InvokeAsync(() =>
            {
                themeColor = theme == Theme.Dark ? ThemeColor.DarkSurface : ThemeColor.LightSurface;
                StateHasChanged();
            });
        }

        private async Task InitThemeAsync()
        {
            ThemeService.ThemeChanged += ThemeChanged;
            await SystemThemeJSModule.Init();
            var theme = SettingService.Get(s => s.Theme);
            ThemeService.SetTheme(theme);
        }

        private Task InitLanguageAsync()
        {
            var language = SettingService.Get(s => s.Language);
            I18n.SetCulture(new(language));
            return Task.CompletedTask;
        }
    }
}
