using Microsoft.AspNetCore.Components.WebView;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Hybird.Extensions;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui
{
    public partial class MainPage : ContentPage
    {
        private readonly Color _backgroundColor;

        private readonly RouteMatcher _routeMatcher;

        private readonly IAppLifecycle _appLifecycle;

        public MainPage(Color backgroundColor,
            RouteMatcher routeMatcher,
            IAppLifecycle appLifecycle)
        {
            InitializeComponent();

            _backgroundColor = backgroundColor;
            _routeMatcher = routeMatcher;
            _appLifecycle = appLifecycle;

            blazorWebView.BlazorWebViewInitializing += BlazorWebViewInitializingCore;
            blazorWebView.BlazorWebViewInitialized += BlazorWebViewInitialized;
#if IOS
            Initialize();
#endif
        }

        private partial void BlazorWebViewInitializing(object? sender, BlazorWebViewInitializingEventArgs e);

        private partial void BlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e);

        private void BlazorWebViewInitializingCore(object? sender, BlazorWebViewInitializingEventArgs e)
        {
            HandleAppActivation();
            BlazorWebViewInitializing(sender, e);
        }

        private void HandleAppActivation()
        {
            // Welcome Page
            bool firstSetLanguage = Microsoft.Maui.Storage.Preferences.Default.Get<bool>(nameof(Setting.FirstSetLanguage), false);
            bool firstAgree = Microsoft.Maui.Storage.Preferences.Default.Get<bool>(nameof(Setting.FirstAgree), false);
            if (!firstSetLanguage || !firstAgree)
            {
                blazorWebView.StartPath = "/welcome";
                _appLifecycle.ActivationArguments = null;
                return;
            }

            var args = _appLifecycle.ActivationArguments;
            // Quick Record
            if (args is null || args.Data is null || args.Kind == AppActivationKind.Launch)
            {
                var quickRecord = Microsoft.Maui.Storage.Preferences.Default.Get<bool>(nameof(Setting.QuickRecord), false);
                if (quickRecord)
                {
                    args = _appLifecycle.ActivationArguments = new()
                    {
                        Kind = AppActivationKind.Scheme,
                        Data = $"{SchemeConstants.SwashbucklerDiary}://write"
                    };
                }
            }

            // App lock
            string appLockNumberPassword = Preferences.Default.Get<string>(nameof(Setting.AppLockNumberPassword), string.Empty);
            string appLockPatternPassword = Preferences.Default.Get<string>(nameof(Setting.AppLockPatternPassword), string.Empty);
            bool appLockBiometric = Preferences.Default.Get<bool>(nameof(Setting.AppLockBiometric), false);
            bool useAppLock = !string.IsNullOrEmpty(appLockNumberPassword)
                || !string.IsNullOrEmpty(appLockPatternPassword)
                || appLockBiometric;
            if (useAppLock)
            {
                blazorWebView.StartPath = "/appLock";
                return;
            }

            if (args is null || args.Data is null)
            {
                return;
            }

            switch (args.Kind)
            {
                case AppActivationKind.Share:
                    HandleShare(args);
                    break;
                case AppActivationKind.Scheme:
                    HandleScheme(args);
                    break;
            }
        }

        private void HandleScheme(ActivationArguments args)
        {
            string? uriString = args.Data as string;
            if (_routeMatcher.CheckUrlScheme(uriString, out var path))
            {
                blazorWebView.StartPath = path;
            }

            _appLifecycle.ActivationArguments = null;
        }

        private void HandleShare(ActivationArguments args)
        {
            blazorWebView.StartPath = "/write";
        }
    }
}