using BlazorComponent;
using BlazorComponent.I18n;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using Theme = SwashbucklerDiary.Shared.Theme;

namespace SwashbucklerDiary.Rcl.Layout
{
    public partial class MainLayoutBase : LayoutComponentBase, IDisposable
    {
        protected StringNumber NavigationButtonIndex = 0;

        protected List<NavigationButton> NavigationButtons = new();

        [Inject]
        protected MasaBlazor MasaBlazor { get; set; } = default!;

        [Inject]
        protected NavigationManager Navigation { get; set; } = default!;

        [Inject]
        protected INavigateService NavigateService { get; set; } = default!;

        [Inject]
        protected II18nService I18n { get; set; } = default!;

        [Inject]
        protected IPreferences Preferences { get; set; } = default!;

        [Inject]
        protected IPopupService PopupService { get; set; } = default!;

        [Inject]
        protected IAlertService AlertService { get; set; } = default!;

        [Inject]
        protected IThemeService ThemeService { get; set; } = default!;

        [Inject]
        protected IVersionUpdataManager VersionManager { get; set; } = default!;

        public void Dispose()
        {
            ThemeService.OnChanged -= ThemeChanged;
            I18n.OnChanged -= StateHasChanged;
            GC.SuppressFinalize(this);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            NavigateService.Initialize(Navigation);
            AlertService.Initialize(PopupService);
            LoadView();
            ThemeService.OnChanged += ThemeChanged;
            I18n.OnChanged += StateHasChanged;
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await VersionManager.UpdateVersion();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await LoadSettings();
                StateHasChanged();
            }
        }

        protected async Task LoadSettings()
        {
            var themeState = await Preferences.Get<int>(Setting.ThemeState);
            await ThemeService.SetThemeAsync((Theme)themeState);
            var language = await Preferences.Get<string>(Setting.Language);
            I18n.Initialize(language);
            var timeout = await Preferences.Get<int>(Setting.AlertTimeout);
            AlertService.SetTimeout(timeout);
        }

        protected void LoadView()
        {
            List<NavigationButton> navigationButtons = [];
            navigationButtons.Add(new(this, navigationButtons.Count, "Main.Diary", "mdi-notebook-outline", "mdi-notebook", GetIcon, () => PopToRootAsync("")));
            navigationButtons.Add(new(this, navigationButtons.Count, "Main.History", "mdi-clock-outline", "mdi-clock", GetIcon, () => PopToRootAsync("history")));
            navigationButtons.Add(new(this, navigationButtons.Count, "Main.Mine", "mdi-account-outline", "mdi-account", GetIcon, () => PopToRootAsync("mine")));
            NavigationButtons = navigationButtons;
        }

        protected Task PopToRootAsync(string url)
            => NavigateService.PopToRootAsync(url);

        protected async Task ThemeChanged(Theme theme)
        {
            if (MasaBlazor.Theme.Dark != (theme == Theme.Dark))
            {
                MasaBlazor.ToggleTheme();
            }

            await InvokeAsync(StateHasChanged);
        }

        protected string GetIcon(NavigationButton navigationButton)
        {
            return navigationButton.Index == NavigationButtonIndex ? navigationButton.SelectedIcon : navigationButton.NotSelectedIcon;
        }
    }
}
