using Microsoft.AspNetCore.Components.WebView;
using Microsoft.AspNetCore.Components.WebView.Gtk;

namespace SwashbucklerDiary.Gtk
{
    public class MainWindow : global::Gtk.ApplicationWindow
    {
        private readonly Gdk.RGBA _backgroundColor;

        public MainWindow(
            global::Gtk.Application application,
            IServiceProvider serviceProvider,
            Gdk.RGBA backgroundColor
            )
            : base(global::Gtk.Internal.ApplicationWindow.New(application.Handle), false)
        {
            _backgroundColor = backgroundColor;

            SetDefaultSize(1024, 768);

            this.OnDestroy += (o, e) =>
            {
                application.Quit();
            };

            var blazorWebView = new Microsoft.AspNetCore.Components.WebView.Gtk.BlazorWebView();
            blazorWebView.HostPage = Path.Combine("wwwroot", "index.html");
            blazorWebView.Services = serviceProvider;
            blazorWebView.RootComponents.Add<Routes>("#app");

            blazorWebView.BlazorWebViewInitialized += BlazorWebViewInitialized;

            this.SetChild(blazorWebView);
        }

        //public static MainWindow New(
        //    global::Gtk.Application application,
        //    IServiceProvider serviceProvider,
        //    Gdk.RGBA backgroundColor)
        //{
        //    var applicationwindowHandle = global::Gtk.Internal.ApplicationWindow.New(application.Handle);
        //    return new MainWindow(applicationwindowHandle, false, application, serviceProvider, backgroundColor);
        //}

        private void BlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e)
        {
            e.WebView.SetBackgroundColor(_backgroundColor);
            var settings = e.WebView.GetSettings();
            settings.UserAgent += " Android Mobile";
            settings.MediaPlaybackRequiresUserGesture = false;
            e.WebView.SetSettings(settings);
        }
    }
}
