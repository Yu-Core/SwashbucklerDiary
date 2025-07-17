using Microsoft.AspNetCore.Components.WebView;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Maui
{
    public partial class MainPage : ContentPage
    {
        private readonly Color _backgroundColor;

        private readonly INavigateController _navigateController;

        public MainPage(Color backgroundColor,
            INavigateController navigateController)
        {
            InitializeComponent();

            _backgroundColor = backgroundColor;
            _navigateController = navigateController;

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
            bool firstSetLanguage = Microsoft.Maui.Storage.Preferences.Default.Get<bool>(nameof(Setting.FirstSetLanguage), false);
            bool firstAgree = Microsoft.Maui.Storage.Preferences.Default.Get<bool>(nameof(Setting.FirstAgree), false);
            if (!firstSetLanguage || !firstAgree)
            {
                blazorWebView.StartPath = "/welcome";
                Essentials.AppLifecycle.Default.ActivationArguments = null;
                return;
            }

            var args = Essentials.AppLifecycle.Default.ActivationArguments;
            if (args is null || args.Data is null || args.Kind == AppActivationKind.Launch)
            {
                QuickRecord();
            }

            string appLockNumberPassword = Preferences.Default.Get<string>(nameof(Setting.AppLockNumberPassword), string.Empty);
            bool appLockBiometric = Preferences.Default.Get<bool>(nameof(Setting.AppLockBiometric), false);
            bool useAppLock = !string.IsNullOrEmpty(appLockNumberPassword) || appLockBiometric;
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
            if (_navigateController.CheckUrlScheme(uriString, out var path))
            {
                blazorWebView.StartPath = path;
            }

            Essentials.AppLifecycle.Default.ActivationArguments = null;
        }

        private void HandleShare(ActivationArguments args)
        {
            blazorWebView.StartPath = "/write";
        }

        private void QuickRecord()
        {
            var quickRecord = Microsoft.Maui.Storage.Preferences.Default.Get<bool>(nameof(Setting.QuickRecord), false);
            if (quickRecord)
            {
                Essentials.AppLifecycle.Default.ActivationArguments = new()
                {
                    Kind = AppActivationKind.Scheme,
                    Data = $"{SchemeConstants.SwashbucklerDiary}://write"
                };
            }
        }
    }
}