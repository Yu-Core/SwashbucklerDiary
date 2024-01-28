using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using SwashbucklerDiary.WebAssembly.Essentials;

namespace SwashbucklerDiary.WebAssembly
{
    public partial class App : IDisposable
    {
        [Inject]
        private SystemThemeJSModule SystemThemeJSModule { get; set; } = default!;

        [Inject]
        private IAppLifecycle AppLifecycle { get; set; } = default!;

        [Inject]
        private IVersionTracking VersionTracking { get; set; } = default!;

        [Inject]
        private IPreferences Preferences { get; set; } = default!;

        [Inject]
        private IThemeService ThemeService { get; set; } = default!;

        [Inject]
        protected MasaBlazor MasaBlazor { get; set; } = default!;

        [Inject]
        private II18nService I18n { get; set; } = default!;

        public void Dispose()
        {
            ThemeService.OnChanged -= ThemeChanged;
            GC.SuppressFinalize(this);
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            ThemeService.OnChanged += ThemeChanged;

            await Task.WhenAll(
                InitTheme(),
                InitLanguage(),
                ((AppLifecycle)AppLifecycle).InitializedAsync(),
                VersionTracking.Track(typeof(App).Assembly)
            );
        }

        private async Task InitTheme()
        {
            await SystemThemeJSModule.InitializedAsync();
            var themeState = await Preferences.Get<int>(Setting.ThemeState);
            await ThemeService.SetThemeAsync((Theme)themeState);
        }

        private async Task InitLanguage()
        {
            var language = await Preferences.Get<string>(Setting.Language);
            I18n.SetCulture(language);
        }

        private Task ThemeChanged(Theme theme)
        {
            if (MasaBlazor.Theme.Dark != (theme == Theme.Dark))
            {
                MasaBlazor.ToggleTheme();
            }

            return Task.CompletedTask;
        }
    }
}
