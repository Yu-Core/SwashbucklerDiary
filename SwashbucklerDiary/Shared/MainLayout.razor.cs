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
        bool ShowFirstLaunch = true;
        StringNumber SelectedItemIndex = 0;
        List<NavigationButton> NavigationButtons = new();

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
        [Inject]
        private ISystemService SystemService { get; set; } = default!;

        public void Dispose()
        {
            MasaBlazor.Breakpoint.OnUpdate -= InvokeStateHasChangedAsync;
            GC.SuppressFinalize(this);
        }

        protected override async Task OnInitializedAsync()
        {
            NavigateService.Initialize(Navigation);
            AlertService.Initialize(PopupService);
            I18nService.Initialize(I18n);
            LoadView();
            MasaBlazor.Breakpoint.OnUpdate += InvokeStateHasChangedAsync;
            ThemeService.OnChanged += ThemeChanged;
            I18nService.OnChanged += StateHasChanged;
            await LoadSettings();
            await base.OnInitializedAsync();
        }

        private bool Mini => MasaBlazor.Breakpoint.Sm;

        private bool ShowBottomNavigation
        {
            get
            {
                if (MasaBlazor.Breakpoint.SmAndUp || ShowFirstLaunch)
                {
                    return false;
                }
                string[] links = { "", "history", "mine" };
                var url = Navigation!.ToBaseRelativePath(Navigation.Uri);
                return links.Any(it => it == url.Split("?")[0]);
            }
        }

        private bool ShowNavigationDrawer
        {
            get
            {
                if (!MasaBlazor.Breakpoint.SmAndUp || ShowFirstLaunch)
                {
                    return false;
                }

                return true;
            }
        }

        private static string MainStyle
        {
            get
            {
                string style = string.Empty;
                style += "transition:padding-left ease 0.3s !important;";
                return style;
            }
        }

        private async Task LoadSettings()
        {
            int themeState = SettingsService.GetDefault<int>(SettingType.ThemeState);
            ThemeService.SetThemeState((ThemeState)themeState);
            var language = await SettingsService.Get<string>(SettingType.Language);
            I18nService.SetCulture(language);
        }

        private void LoadView()
        {
            NavigationButtons = new()
            {
                new ( "Main.Diary", "mdi-notebook-outline", "mdi-notebook", ()=>To("")),
                new ( "Main.History", "mdi-clock-outline", "mdi-clock", ()=>To("history")),
                new ( "Main.Mine", "mdi-account-outline", "mdi-account", ()=>To("mine"))
            };
        }

        private async Task InvokeStateHasChangedAsync()
        {
            await InvokeAsync(StateHasChanged);
        }

        protected async void To(string url)
        {
            await NavigateService.NavBtnClick();
            Navigation.NavigateTo(url);
        }

        private void ThemeChanged(ThemeState state)
        {
            if (MasaBlazor.Theme.Dark != (state == ThemeState.Dark))
            {
                MasaBlazor.ToggleTheme();
            }

            SystemService.SetStatusBar(state);
            InvokeAsync(StateHasChanged);
        }
    }
}
