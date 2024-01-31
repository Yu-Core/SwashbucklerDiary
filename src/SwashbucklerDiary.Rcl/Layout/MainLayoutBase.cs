using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Layout
{
    public partial class MainLayoutBase : LayoutComponentBase, IDisposable
    {
        protected List<NavigationButton> NavigationButtons = [
            new("Main.Diary", "mdi-notebook-outline", "mdi-notebook", ""),
            new("Main.History", "mdi-clock-outline", "mdi-clock", "history"),
            new("Main.Mine",  "mdi-account-outline", "mdi-account", "mine"),
        ];

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

        [Inject]
        protected MasaBlazor MasaBlazor { get; set; } = default!;

        public void Dispose()
        {
            OnDispose();
            GC.SuppressFinalize(this);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            LoadView();
            NavigateService.Initialize(Navigation, NavigationButtons.Select(it => it.Href).ToList());
            AlertService.Initialize(PopupService);
            I18n.OnChanged += StateHasChanged;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await UpdateSettings();
                StateHasChanged();
            }
        }

        protected virtual void OnDispose()
        {
            I18n.OnChanged -= StateHasChanged;
        }

        protected async Task UpdateSettings()
        {
            var timeout = await Preferences.Get<int>(Setting.AlertTimeout);
            AlertService.SetTimeout(timeout);
        }

        protected void LoadView()
        {
            for (int i = 0; i < NavigationButtons.Count; i++)
            {
                var button = NavigationButtons[i];
                button.OnClick = () => NavigateService.PopToRootAsync(button.Href);
            }
        }

        protected Task ThemeChanged(Theme theme)
        {
            if (MasaBlazor.Theme.Dark != (theme == Theme.Dark))
            {
                MasaBlazor.ToggleTheme();
            }

            return Task.CompletedTask;
        }
    }
}
