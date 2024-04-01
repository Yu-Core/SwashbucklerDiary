using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using System.Globalization;

namespace SwashbucklerDiary.Rcl.Layout
{
    public partial class MainLayoutBase : LayoutComponentBase, IDisposable
    {
        protected bool afterInitSetting;

        protected List<NavigationButton> NavigationButtons = [
            new("Main.Diary", "mdi-notebook-outline", "mdi-notebook", ""),
            new("Main.History", "mdi-clock-outline", "mdi-clock", "history"),
            new("Main.FileBrowse", "mdi-file-outline", "mdi-file", "fileBrowse"),
            new("Main.Mine",  "mdi-account-outline", "mdi-account", "mine"),
        ];

        [Inject]
        protected NavigationManager Navigation { get; set; } = default!;

        [Inject]
        protected INavigateService NavigateService { get; set; } = default!;

        [Inject]
        protected II18nService I18n { get; set; } = default!;

        [Inject]
        protected ISettingService SettingService { get; set; } = default!;

        [Inject]
        protected IPopupService PopupService { get; set; } = default!;

        [Inject]
        protected IAlertService AlertService { get; set; } = default!;

        [Inject]
        protected IThemeService ThemeService { get; set; } = default!;

        [Inject]
        protected IVersionUpdataManager VersionUpdataManager { get; set; } = default!;

        [Inject]
        protected MasaBlazor MasaBlazor { get; set; } = default!;

        [Inject]
        protected IJSRuntime JSRuntime { get; set; } = default!;

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
            I18n.OnChanged += LanguageChanged;
        }

        protected virtual void OnDispose()
        {
            I18n.OnChanged -= LanguageChanged;
        }

        protected virtual async Task InitSettingsAsync()
        {
            await SettingService.InitializeAsync();
            afterInitSetting = true;
            var timeout = SettingService.Get<int>(Setting.AlertTimeout);
            AlertService.SetTimeout(timeout);
        }

        protected void LoadView()
        {
            foreach (var button in NavigationButtons)
            {
                button.OnClick = () => NavigateService.PopToRootAsync(button.Href);
            }
        }

        protected void ThemeChanged(Theme theme)
        {
            if (MasaBlazor.Theme.Dark != (theme == Theme.Dark))
            {
                MasaBlazor.ToggleTheme();
            }
        }

        protected async void LanguageChanged(CultureInfo cultureInfo)
        {
            StateHasChanged();
            await JSRuntime.InvokeVoidAsync("changeLanguage", cultureInfo.Name);
        }
    }
}
