using BlazorComponent;
using BlazorComponent.I18n;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Shared
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
        private I18n I18n { get; set; } = default!;

        [Inject]
        private II18nService I18nService { get; set; } = default!;

        [Inject]
        private ISettingsService SettingsService { get; set; } = default!;

        [Inject]
        private IPopupService PopupService { get; set; } = default!;

        [Inject]
        private IAlertService AlertService { get; set; } = default!;

        [Inject]
        private IThemeService ThemeService { get; set; } = default!;

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
            I18nService.Initialize(I18n);
            LoadView();
            SetRootPath();
            ThemeService.OnChanged += ThemeChanged;
            I18nService.OnChanged += StateHasChanged;
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await base.OnInitializedAsync();
        }

        private async Task LoadSettings()
        {
            var themeState = await SettingsService.Get<int>(SettingType.ThemeState);
            ThemeService.SetThemeState((ThemeState)themeState);
            var language = await SettingsService.Get<string>(SettingType.Language);
            I18nService.SetCulture(language);
            var timeout = await SettingsService.Get<int>(SettingType.AlertTimeout);
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

        private void ThemeChanged(ThemeState state)
        {
            if (MasaBlazor.Theme.Dark != (state == ThemeState.Dark))
            {
                MasaBlazor.ToggleTheme();
            }

            InvokeAsync(StateHasChanged);
        }

        private void SetRootPath()
        {
            NavigateService.RootPaths.Add(Navigation.Uri);
        }
    }
}
