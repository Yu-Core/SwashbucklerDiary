using Microsoft.AspNetCore.Components.WebView;
using SwashbucklerDiary.Maui.Essentials;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;

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
            var args = AppActivation.Arguments;
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
                default:
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
    }
}