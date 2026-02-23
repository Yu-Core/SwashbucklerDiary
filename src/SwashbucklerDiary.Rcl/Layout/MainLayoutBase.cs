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
        protected bool afterInitConfig;

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
        protected IAppLifecycle AppLifecycle { get; set; } = default!;

        [Inject]
        protected IPlatformIntegration PlatformIntegration { get; set; } = default!;

        [Inject]
        protected IAlertService AlertService { get; set; } = default!;

        [Inject]
        protected IAppLockService AppLockService { get; set; } = default!;

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

            I18n.CultureChanged += HandleLanguageChanged;
            ThemeService.OnChanged += HandleThemeChanged;
            SettingService.SettingsChanged += HandleSettingsChanged;
            AppLifecycle.OnStopped += HandleAppLifecycleOnStopped;
            AppLifecycle.OnActivated += HandleActivated;
            NavigateController.OnBackPressed += HandleBackPressed;
            AppLockService.ValidationSucceeded += HandleValidationSucceeded;
        }

        private Task HandleValidationSucceeded(AppLockEventArgs args)
        {
            if (!args.IsAppLaunch)
            {
                return Task.CompletedTask;
            }

            InvokeAsync(DialogNotificationCoreAsync);
            return Task.CompletedTask;
        }

        protected abstract void HandleSchemeActivation(ActivationArguments args, bool replace);

        protected virtual void OnDispose()
        {
            I18n.CultureChanged -= HandleLanguageChanged;
            ThemeService.OnChanged -= HandleThemeChanged;
            SettingService.SettingsChanged -= HandleSettingsChanged;
            AppLifecycle.OnStopped -= HandleAppLifecycleOnStopped;
            AppLifecycle.OnActivated -= HandleActivated;
            NavigateController.OnBackPressed -= HandleBackPressed;
            AppLockService.ValidationSucceeded -= HandleValidationSucceeded;
        }

        protected async Task InternalOnInitializedAsync()
        {
            await InitVersionUpdate();
            await InitConfigAsync();
            StateHasChanged();

            await DialogNotificationAsync();
        }

        protected virtual async Task InitConfigAsync()
        {
            await GlobalConfiguration.InitializeAsync();
            afterInitConfig = true;
        }

        protected void HandleLanguageChanged(object? sender, EventArgs e)
        {
            InvokeAsync(async () =>
            {
                MasaBlazor.RTL = I18n.Culture.TextInfo.IsRightToLeft;
                await UpdateDocumentProperty(I18n.Culture);
                StateHasChanged();
            });
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

        protected async Task DialogNotificationAsync()
        {
            var route = NavigationManager.GetRoute();
            if (route == "/welcome" || route == "/appLock")
            {
                return;
            }

            await DialogNotificationCoreAsync();
        }

        protected virtual Task DialogNotificationCoreAsync()
        {
            return Task.CompletedTask;
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
                var returnUrl = NavigationManager.GetBaseRelativePath();
                NavigationManager.NavigateTo($"appLock?IsAppLaunch=false&returnUrl={Uri.EscapeDataString(returnUrl)}");
            }
        }

        protected void HandleActivated(ActivationArguments? args)
        {
            var route = NavigationManager.GetRoute();
            if (route == "/welcome")
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

            bool replace = route == "/appLock";

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
            var route = NavigationManager.GetRoute();
            if (route == "/write")
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

        protected virtual async Task InitVersionUpdate()
        {
            AlertService.StartLoading(I18n.T("Upgrading and optimizing in progress. Please wait..."));
            await VersionUpdataManager.HandleVersionUpdate();
            AlertService.StopLoading();
        }
    }
}
