using Microsoft.AspNetCore.Components.WebView;
using Microsoft.AspNetCore.Components.WebView.Gtk;
using Microsoft.Extensions.DependencyInjection;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Hybird.Extensions;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Gtk
{
    public class MainWindow : global::Gtk.ApplicationWindow
    {
        private readonly Gdk.RGBA _backgroundColor;

        private readonly RouteMatcher _routeMatcher;

        private readonly Microsoft.AspNetCore.Components.WebView.Gtk.BlazorWebView blazorWebView;

        private readonly IAppLifecycle _appLifecycle;

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
            _routeMatcher = serviceProvider.GetRequiredService<RouteMatcher>();
            _appLifecycle = serviceProvider.GetRequiredService<IAppLifecycle>();
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
            string appLockNumberPassword = Microsoft.Maui.Storage.Preferences.Default.Get<string>(nameof(Setting.AppLockNumberPassword), string.Empty);
            string appLockPatternPassword = Microsoft.Maui.Storage.Preferences.Default.Get<string>(nameof(Setting.AppLockPatternPassword), string.Empty);
            bool useAppLock = !string.IsNullOrEmpty(appLockNumberPassword)
                || !string.IsNullOrEmpty(appLockPatternPassword);
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
    }
}
