using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui
{
    public partial class Routes : IDisposable
    {
        [Inject]
        private Rcl.Essentials.IPreferences Preferences { get; set; } = default!;

        [Inject]
        private IThemeService ThemeService { get; set; } = default!;

        [Inject]
        protected MasaBlazor MasaBlazor { get; set; } = default!;

        public void Dispose()
        {
            ThemeService.OnChanged -= ThemeChanged;
            GC.SuppressFinalize(this);
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            ThemeService.OnChanged += ThemeChanged;
            await UpdateSettings();
        }

        private async Task UpdateSettings()
        {
            var themeState = await Preferences.Get<int>(Setting.ThemeState);
            await ThemeService.SetThemeAsync((Theme)themeState);
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
