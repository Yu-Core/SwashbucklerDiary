using Microsoft.AspNetCore.Components.WebView;
using Microsoft.AspNetCore.Components.WebView.Gtk;
using Microsoft.Extensions.DependencyInjection;
using SwashbucklerDiary.Gtk.BlazorWebView;
using SwashbucklerDiary.Gtk.Essentials;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Services;

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
            blazorWebView.BlazorWebViewWebResourceRequested += BlazorWebViewWebResourceRequested;
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

        private void BlazorWebViewWebResourceRequested(object? sender, BlazorWebViewWebResourceRequestedEventArgs e)
        {
            e.Handled = LocalFileWebAccessHelper.InterceptCustomPathRequest(e.Request);
        }

        private void HandleAppActivation()
        {
            bool firstSetLanguage = Preferences.Default.Get<bool>(nameof(Setting.FirstSetLanguage), false);
            bool firstAgree = Preferences.Default.Get<bool>(nameof(Setting.FirstAgree), false);
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

        private void QuickRecord()
        {
            var settingService = blazorWebView.Services.GetRequiredService<ISettingService>();
            var quickRecord = settingService.Get(it => it.QuickRecord);
            if (quickRecord)
            {
                blazorWebView.StartPath = "/write";
            }
        }
    }
}
