using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using System.Globalization;

namespace SwashbucklerDiary.Rcl.Layout
{
    public abstract partial class MainLayoutBase : LayoutComponentBase, IDisposable
    {
        protected bool afterInitSetting;

        protected bool showSponsorSupport;

        protected readonly List<NavigationButton> navigationButtons = [
            new("Main.Diary", "mdi-notebook-outline", "mdi-notebook", ""),
            new("Main.History", "mdi-clock-outline", "mdi-clock", "history"),
            new("Main.FileBrowse", "mdi-file-outline", "mdi-file", "fileBrowse"),
            new("Main.Mine",  "mdi-account-outline", "mdi-account", "mine"),
        ];

        protected IEnumerable<string> permanentPaths = [];

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected INavigateController NavigateController { get; set; } = default!;

        [Inject]
        protected II18nService I18n { get; set; } = default!;

        [Inject]
        protected ISettingService SettingService { get; set; } = default!;

        [Inject]
        protected IVersionUpdataManager VersionUpdataManager { get; set; } = default!;

        [Inject]
        protected MasaBlazor MasaBlazor { get; set; } = default!;

        [Inject]
        protected IJSRuntime JSRuntime { get; set; } = default!;

        [Inject]
        protected IGlobalConfiguration GlobalConfiguration { get; set; } = default!;

        [Inject]
        protected IThemeService ThemeService { get; set; } = default!;

        public void Dispose()
        {
            OnDispose();
            GC.SuppressFinalize(this);
        }

        protected bool IsPermanentPath
            => permanentPaths.Any(it => it == NavigationManager.GetAbsolutePath());

        protected override void OnInitialized()
        {
            base.OnInitialized();

            permanentPaths = navigationButtons.Select(it => NavigationManager.ToAbsoluteUri(it.Href).AbsolutePath).ToList();
            NavigateController.Init(NavigationManager, JSRuntime, permanentPaths);

            I18n.OnChanged += HandleLanguageChanged;
            SettingService.SettingsChanged += HandleSettingsChanged;
            VersionUpdataManager.AfterCheckFirstLaunch += HandleSponsorSupport;
        }

        protected virtual void OnDispose()
        {
            I18n.OnChanged -= HandleLanguageChanged;
            SettingService.SettingsChanged -= HandleSettingsChanged;
            VersionUpdataManager.AfterCheckFirstLaunch -= HandleSponsorSupport;
        }

        protected virtual async Task InitSettingsAsync()
        {
            await SettingService.InitializeAsync();
            await GlobalConfiguration.InitializeAsync();
            afterInitSetting = true;
        }

        protected async void HandleLanguageChanged(CultureInfo cultureInfo)
        {
            await InvokeAsync(StateHasChanged);
            await JSRuntime.EvaluateJavascript($"document.documentElement.lang = '{cultureInfo.Name}';");
        }

        protected void HandleSettingsChanged()
        {
            var theme = SettingService.Get(it => it.Theme);
            ThemeService.SetTheme(theme);
            // A bug, it will cause the language selection RadioDialog to fail to select
            //var language = SettingService.Get(it => it.Language);
            //I18n.SetCulture(language);
        }

        private async void HandleSponsorSupport()
        {
            DateTime currentTime = DateTime.Now;
            if (currentTime.Month == 1)
            {
                string key = "LastShowForSponsorSupport";
                DateTime lastShowTime = SettingService.Get(key, DateTime.MinValue);

                if (currentTime.Year != lastShowTime.Year)
                {
                    showSponsorSupport = true;
                    await InvokeAsync(StateHasChanged);
                    await SettingService.SetAsync(key, currentTime);
                }
            }
        }
    }
}
