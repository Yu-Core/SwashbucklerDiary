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
            new("Diary", "book", ""),
            new("Calendar", "schedule", "history"),
            new("File", "draft", "fileBrowse"),
            new("Mine",  "person", "mine"),
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
            _ = UpdateDocumentProperty(I18n.Culture);

            I18n.CultureChanged += HandleLanguageChanged;
            ThemeService.OnChanged += HandleThemeChanged;
            SettingService.SettingsChanged += HandleSettingsChanged;
            VersionUpdataManager.AfterCheckFirstLaunch += HandleSponsorSupport;
        }

        protected virtual void OnDispose()
        {
            I18n.CultureChanged -= HandleLanguageChanged;
            ThemeService.OnChanged -= HandleThemeChanged;
            SettingService.SettingsChanged -= HandleSettingsChanged;
            VersionUpdataManager.AfterCheckFirstLaunch -= HandleSponsorSupport;
        }

        protected virtual async Task InitSettingsAsync()
        {
            await SettingService.InitializeAsync();
            await GlobalConfiguration.InitializeAsync();
            afterInitSetting = true;
        }

        protected async void HandleLanguageChanged(object? sender, EventArgs e)
        {
            await InvokeAsync(StateHasChanged);
            await UpdateDocumentProperty(I18n.Culture);
        }

        protected async void HandleThemeChanged(Shared.Theme theme)
        {
            string mode = theme == Shared.Theme.Dark ? "dark" : "light";
            await JSRuntime.EvaluateJavascript($"Vditor.setContentTheme('{mode}', '_content/{StaticWebAssets.RclAssemblyName}/npm/vditor/3.11.1/dist/css/content-theme');");
        }

        protected async Task UpdateDocumentProperty(CultureInfo cultureInfo)
        {
            // When html lang is not English, the vertical position of Chinese characters and icons cannot be aligned
            //await JSRuntime.EvaluateJavascript($"document.documentElement.lang = '{cultureInfo.Name}';");
            await JSRuntime.EvaluateJavascript($"document.title = '{I18n.T("Swashbuckler Diary")}';");
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
