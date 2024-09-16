using Microsoft.AspNetCore.Components.WebView;
using SwashbucklerDiary.Maui.BlazorWebView;
using SwashbucklerDiary.Maui.Essentials;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Maui
{
    public partial class MainPage : ContentPage
    {
        private readonly Color _backgroundColor;

        public MainPage(Color backgroundColor)
        {
            InitializeComponent();

            _backgroundColor = backgroundColor;

            blazorWebView.BlazorWebViewInitializing += BlazorWebViewInitializingCore;
            blazorWebView.BlazorWebViewInitialized += BlazorWebViewInitialized;
#if IOS || MACCATALYST
            blazorWebView.Loaded += BlazorWebViewLoaded;
#endif

#if IOS
            Initialize();
#endif
        }
#if IOS || MACCATALYST
        private void BlazorWebViewLoaded(object? sender, EventArgs e)
        {
            if (wKWebView is not null)
            {
                wKWebView.NavigationDelegate = new WebViewNavigationDelegate();
            }
        }
#endif
        private partial void BlazorWebViewInitializing(object? sender, BlazorWebViewInitializingEventArgs e);

        private partial void BlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e);

        private void BlazorWebViewInitializingCore(object? sender, BlazorWebViewInitializingEventArgs e)
        {
            HandleLaunchActivation();
            BlazorWebViewInitializing(sender, e);
        }

        private void HandleLaunchActivation()
        {
            var args = LaunchActivation.ActivationArguments;
            if (args is not null)
            {
                switch (args.Kind)
                {
                    case LaunchActivationKind.Share:
                        HandleShare();
                        break;
                    case LaunchActivationKind.Scheme:
                        HandleScheme();
                        break;
                    default:
                        break;
                }
            }
        }

        private void HandleScheme()
        {
            string? uriString = LaunchActivation.ActivationArguments?.Data as string;
            if (Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
            {
                blazorWebView.StartPath = uri.AbsolutePath;
            }

            LaunchActivation.ActivationArguments = null;
        }

        private void HandleShare()
        {
            blazorWebView.StartPath = "/write";
        }
    }
}