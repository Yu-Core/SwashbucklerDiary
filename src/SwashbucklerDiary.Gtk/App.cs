using Gio;
using Microsoft.Extensions.DependencyInjection;
using SwashbucklerDiary.Gtk.Essentials;
using SwashbucklerDiary.Rcl;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Gtk
{
    public class App
    {
        private static readonly string applicationId = AppInfo.PackageName;

        private readonly global::Gtk.Application app;

        private readonly IServiceProvider _serviceProvider;

        private readonly IThemeService _themeService;

        private readonly Masa.Blazor.MasaBlazor _masaBlazor;

        private readonly IAppLifecycle _appLifecycle;

        private global::Gtk.ApplicationWindow? window;

        private WindowLifecycleHelper? windowLifecycleHelper;

        private Gdk.RGBA backgroundColor = default!;

        public App(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            var i18n = _serviceProvider.GetRequiredService<Masa.Blazor.I18n>();

            app = global::Gtk.Application.New(applicationId, Gio.ApplicationFlags.NonUnique);
            GLib.Functions.SetPrgname("SwashbucklerDiary");
            // Set the human-readable application name for app bar and task list.
            GLib.Functions.SetApplicationName(i18n.T("AppName"));
            app.OnActivate += ApplicationActivate;

            _themeService = _serviceProvider.GetRequiredService<IThemeService>();
            _masaBlazor = _serviceProvider.GetRequiredService<Masa.Blazor.MasaBlazor>();
            _appLifecycle = _serviceProvider.GetRequiredService<IAppLifecycle>();
            InitTheme();
        }

        private void ApplicationActivate(Application sender, EventArgs args)
        {
            window = new MainWindow(app, _serviceProvider, backgroundColor);

            windowLifecycleHelper = new WindowLifecycleHelper(window);
            windowLifecycleHelper.Resumed += (s, e) => _appLifecycle.OnResume();
            windowLifecycleHelper.Stopped += (s, e) => _appLifecycle.OnStop();

            window.Present();
        }

        public int Run(string[] args)
        {
            try
            {
                string[] argv = [applicationId, .. args];
                return app.RunWithSynchronizationContext(argv);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                return -1;
            }
        }

        private void InitTheme()
        {
            _themeService.OnChanged += ThemeChanged;

            var themeInt = Preferences.Default.Get<int>(nameof(Setting.Theme), 0);
            var theme = (Theme)themeInt;
            _themeService.SetTheme(theme);

            bool dark = _themeService.RealTheme == Shared.Theme.Dark;
            backgroundColor = new Gdk.RGBA();
            backgroundColor.Parse(dark ? ThemeColor.DarkSurface : ThemeColor.LightSurface);
        }

        private void ThemeChanged(Theme theme)
        {
            _masaBlazor.SetTheme(theme == Theme.Dark);
        }
    }
}
