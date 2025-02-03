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
        private readonly global::Gtk.Application app;

        private readonly IServiceProvider _serviceProvider;

        private readonly IThemeService _themeService;

        private readonly Masa.Blazor.MasaBlazor _masaBlazor;

        private Gdk.RGBA backgroundColor = default!;

        public App(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            var i18n = _serviceProvider.GetRequiredService<Masa.Blazor.I18n>();
            
            app = global::Gtk.Application.New(null, Gio.ApplicationFlags.NonUnique);
            GLib.Functions.SetPrgname("SwashbucklerDiary");
            // Set the human-readable application name for app bar and task list.
            GLib.Functions.SetApplicationName(i18n.T("AppName"));
            app.OnActivate += Activate;

            _themeService = _serviceProvider.GetRequiredService<IThemeService>();
            _masaBlazor = _serviceProvider.GetRequiredService<Masa.Blazor.MasaBlazor>();
            InitTheme();
        }

        private void Activate(Application sender, EventArgs args)
        {
            var window = new MainWindow(app, _serviceProvider, backgroundColor);
            window.Show();
        }

        public void Run()
        {
            app.RunWithSynchronizationContext(null);
        }

        private void InitTheme()
        {
            _themeService.OnChanged += ThemeChanged;

            var themeInt = Preferences.Default.Get<int>(nameof(Setting.Theme), 0);
            var theme = (Theme)themeInt;
            _themeService.SetTheme(theme);

            bool dark = _themeService.RealTheme == Shared.Theme.Dark;
            var rgba = new Gdk.RGBA();
            rgba.Parse(dark ? ThemeColor.DarkSurface : ThemeColor.LightSurface);
            backgroundColor = rgba;
        }

        private void ThemeChanged(Theme theme)
        {
            _masaBlazor.SetTheme(theme == Theme.Dark);
        }
    }
}
