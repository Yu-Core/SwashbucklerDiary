using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.AspNetCore.Components.WebView.Gtk;
using Microsoft.Extensions.DependencyInjection;
using SwashbucklerDiary.Gtk.Essentials;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;

namespace SwashbucklerDiary.Gtk
{
    public class MainWindow : global::Gtk.ApplicationWindow
    {
        private readonly Gdk.RGBA _backgroundColor;

        private readonly INavigateController _navigateController;

        private readonly Microsoft.AspNetCore.Components.WebView.Gtk.BlazorWebView blazorWebView;

        public MainWindow(
            global::Gtk.Application application,
            IServiceProvider serviceProvider,
            Gdk.RGBA backgroundColor
            )
            : base(new global::Gtk.Internal.ApplicationWindowHandle(global::Gtk.Internal.ApplicationWindow.New(application.Handle.DangerousGetHandle()), false))
        {
            SetDefaultSize(1024, 768);

            this.OnDestroy += (o, e) =>
            {
                application.Quit();
            };

            blazorWebView = new();
            blazorWebView.HostPage = Path.Combine("wwwroot", "index.html");
            blazorWebView.Services = serviceProvider;
            blazorWebView.RootComponents.Add<Routes>("#app");

            this.SetChild(blazorWebView);

            _backgroundColor = backgroundColor;
            _navigateController = serviceProvider.GetRequiredService<INavigateController>();
            blazorWebView.BlazorWebViewInitializing += BlazorWebViewInitializing;
            blazorWebView.BlazorWebViewInitialized += BlazorWebViewInitialized;
        }

        private void BlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e)
        {
            e.WebView.SetBackgroundColor(_backgroundColor);
            var settings = e.WebView.GetSettings();
            settings.UserAgent += " Android Mobile";
            settings.MediaPlaybackRequiresUserGesture = false;
            e.WebView.SetSettings(settings);
        }

        private void BlazorWebViewInitializing(object? sender, BlazorWebViewInitializingEventArgs e)
        {
            HandleAppActivation();
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
    }
}
