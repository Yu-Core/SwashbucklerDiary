using Microsoft.AspNetCore.Components.WebView;
using SwashbucklerDiary.Maui.Essentials;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Maui
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

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