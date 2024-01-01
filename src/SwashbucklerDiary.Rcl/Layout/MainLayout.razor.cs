using BlazorComponent;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using Theme = SwashbucklerDiary.Shared.Theme;

namespace SwashbucklerDiary.Rcl.Layout
{
    public partial class MainLayout : IDisposable
    {
        private StringNumber NavigationIndex = 0;

        private List<NavigationButton> NavigationButtons = new();

        [Inject]
        private MasaBlazor MasaBlazor { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        [Inject]
        private INavigateService NavigateService { get; set; } = default!;

        [Inject]
        private II18nService I18nService { get; set; } = default!;

        [Inject]
        private IPreferences Preferences { get; set; } = default!;

        [Inject]
        private IPopupService PopupService { get; set; } = default!;

        [Inject]
        private IAlertService AlertService { get; set; } = default!;

        [Inject]
        private IThemeService ThemeService { get; set; } = default!;

        [Inject]
        private IVersionUpdataManager VersionManager { get; set; } = default!;

        public void Dispose()
        {
            ThemeService.OnChanged -= ThemeChanged;
            I18nService.OnChanged -= StateHasChanged;
            GC.SuppressFinalize(this);
        }

        protected override void OnInitialized()
        {
            NavigateService.Initialize(Navigation);
            AlertService.Initialize(PopupService);
            LoadView();
            ThemeService.OnChanged += ThemeChanged;
            I18nService.OnChanged += StateHasChanged;
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            await VersionManager.UpdateVersion();
            await base.OnInitializedAsync();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if(firstRender)
            {
                await LoadSettings();
            }
        }

        private async Task LoadSettings()
        {
            var themeState = await Preferences.Get<int>(Setting.ThemeState);
            await ThemeService.SetThemeAsync((Theme)themeState);
            var language = await Preferences.Get<string>(Setting.Language);
            I18nService.SetCulture(language);
            var timeout = await Preferences.Get<int>(Setting.AlertTimeout);
            AlertService.SetTimeout(timeout);
        }

        private void LoadView()
        {
            NavigationButtons = new()
            {
                new (this, "Main.Diary", "mdi-notebook-outline", "mdi-notebook", ()=>PopToRootAsync("")),
                new (this, "Main.History", "mdi-clock-outline", "mdi-clock", ()=>PopToRootAsync("history")),
                new (this, "Main.Mine", "mdi-account-outline", "mdi-account", ()=>PopToRootAsync("mine"))
            };
        }

        protected Task PopToRootAsync(string url)
            => NavigateService.PopToRootAsync(url);

        private async Task ThemeChanged(Theme theme)
        {
            if (MasaBlazor.Theme.Dark != (theme == Theme.Dark))
            {
                MasaBlazor.ToggleTheme();
            }

            await InvokeAsync(StateHasChanged);
        }

    }
}
