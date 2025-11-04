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

        [Inject]
        private IAppLifecycle AppLifecycle { get; set; } = default!;

        [Inject]
        private IPlatformIntegration PlatformIntegration { get; set; } = default!;

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

            _ = UpdateDocumentProperty(I18n.Culture);

            I18n.CultureChanged += HandleLanguageChanged;
            ThemeService.OnChanged += HandleThemeChanged;
            SettingService.SettingsChanged += HandleSettingsChanged;
            AppLifecycle.OnStopped += HandleAppLifecycleOnStopped;
            AppLifecycle.OnActivated += HandleActivated;
            NavigateController.OnBackPressed += HandleBackPressed;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                await SponsorSupport();
            }
        }

        protected abstract ActivationArguments CreateAppLockActivationArguments();

        protected abstract void HandleSchemeActivation(ActivationArguments args, bool replace);

        protected virtual void OnDispose()
        {
            I18n.CultureChanged -= HandleLanguageChanged;
            ThemeService.OnChanged -= HandleThemeChanged;
            SettingService.SettingsChanged -= HandleSettingsChanged;
            AppLifecycle.OnStopped -= HandleAppLifecycleOnStopped;
            AppLifecycle.OnActivated -= HandleActivated;
            NavigateController.OnBackPressed -= HandleBackPressed;
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
            await JSRuntime.EvaluateJavascript($"Vditor.setContentTheme('{mode}', '_content/{StaticWebAssets.RclAssemblyName}/npm/vditor@3.11.2/dist/css/content-theme');");
        }

        protected async Task UpdateDocumentProperty(CultureInfo cultureInfo)
        {
            // When html lang is not English, the vertical position of Chinese characters and icons cannot be aligned
            //await JSRuntime.EvaluateJavascript($"document.documentElement.lang = '{cultureInfo.Name}';");
            await JSRuntime.EvaluateJavascript($"document.title = '{I18n.T("Swashbuckler Diary")}';");
        }

        protected void HandleSettingsChanged()
        {
            InvokeAsync(() =>
            {
                var theme = SettingService.Get(it => it.Theme);
                ThemeService.SetTheme(theme);

                var language = SettingService.Get(it => it.Language);
                I18n.SetCulture(new(language));
            });
        }

        private async Task SponsorSupport()
        {
            if (NavigationManager.GetBaseRelativePath().Equals("welcome", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

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

        private async void HandleAppLifecycleOnStopped()
        {
            if (NavigateController.DisableNavigate)
            {
                return;
            }

            string appLockNumberPassword = SettingService.Get(it => it.AppLockNumberPassword);
            string appLockPatternPassword = SettingService.Get(it => it.AppLockPatternPassword);
            bool appLockBiometric = SettingService.Get(it => it.AppLockBiometric);
            bool isBiometricSupported = await PlatformIntegration.IsBiometricSupported();
            bool useAppLock = !string.IsNullOrEmpty(appLockNumberPassword)
                || !string.IsNullOrEmpty(appLockPatternPassword)
                || (appLockBiometric && isBiometricSupported);
            bool lockAppWhenLeave = SettingService.Get(it => it.LockAppWhenLeave);
            if (useAppLock && lockAppWhenLeave)
            {
                AppLifecycle.ActivationArguments = CreateAppLockActivationArguments();
                NavigationManager.NavigateTo("appLock?IsLeave=true");
            }
        }

        protected void HandleActivated(ActivationArguments? args)
        {
            if (NavigationManager.GetBaseRelativePath().Equals("welcome", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            if (args is null || args.Data is null)
            {
                return;
            }

            if (NavigateController.DisableNavigate)
            {
                AppLifecycle.ActivationArguments = args;
                return;
            }

            bool replace = NavigationManager.GetBaseRelativePath().Equals("applock", StringComparison.InvariantCultureIgnoreCase);

            switch (args.Kind)
            {
                case AppActivationKind.Share:
                    HandleShareActivation(args, replace);
                    break;
                case AppActivationKind.Scheme:
                    HandleSchemeActivation(args, replace);
                    break;
                default:
                    break;
            }
        }

        protected void HandleShareActivation(ActivationArguments args, bool replace)
        {
            if (NavigationManager.GetBaseRelativePath().Equals("write", StringComparison.InvariantCultureIgnoreCase))
            {
                return;
            }

            AppLifecycle.ActivationArguments = args;
            To("write", replace: replace);
        }

        protected void To(string uri, bool replace)
        {
            NavigationManager.NavigateTo(uri, new NavigationOptions()
            {
                ReplaceHistoryEntry = replace,
                HistoryEntryState = replace ? "replace" : null
            });
        }

        private async void HandleBackPressed()
        {
            await JSRuntime.HistoryBack();
        }
    }
}
