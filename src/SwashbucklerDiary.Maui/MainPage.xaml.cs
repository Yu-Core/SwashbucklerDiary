using Microsoft.AspNetCore.Components.WebView;
using SwashbucklerDiary.Maui.Essentials;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Services;

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
                AppActivation.Arguments = null;
                return;
            }

            var args = AppActivation.Arguments;
            if (args is null || args.Data is null)
            {
                HandleDefault();
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
                default:
                    HandleDefault();
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

            AppActivation.Arguments = null;
        }

        private void HandleShare(ActivationArguments args)
        {
            blazorWebView.StartPath = "/write";
        }

        private void HandleDefault()
        {
            QuickRecord();
        }

        private void QuickRecord()
        {
            var quickRecord = Microsoft.Maui.Storage.Preferences.Default.Get<bool>(nameof(Setting.QuickRecord), false);
            if (quickRecord)
            {
                blazorWebView.StartPath = "/write";
            }
        }
    }
}