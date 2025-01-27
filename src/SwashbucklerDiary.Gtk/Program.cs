using Microsoft.AspNetCore.Components.WebView.Gtk;
using Microsoft.Extensions.DependencyInjection;
using SwashbucklerDiary.Gtk;
using SwashbucklerDiary.Gtk.Extensions;

#pragma warning disable CS0162 // Unreachable code detected

var app = Gtk.Application.New(null, Gio.ApplicationFlags.NonUnique);
GLib.Functions.SetPrgname("SwashbucklerDiary");
// Set the human-readable application name for app bar and task list.
GLib.Functions.SetApplicationName("SwashbucklerDiary");
app.OnActivate += (s, e) =>
{
    // Create the parent window
    var window = Gtk.ApplicationWindow.New(app);
    window.SetDefaultSize(1024, 768);

    window.OnDestroy += (o, e) =>
    {
        app.Quit();
    };

    // Add the BlazorWebViews
    var services = new ServiceCollection();
    services.AddGtkBlazorWebView();
#if DEBUG
    services.AddBlazorWebViewDeveloperTools();
#endif

    services.AddMasaBlazorConfig();

    services.AddSerilogConfig();

    services.AddSqlsugarConfig();

    services.AddDependencyInjection();

    var blazorWebView = new BlazorWebView();
    blazorWebView.HostPage = Path.Combine("wwwroot", "index.html");
    blazorWebView.Services = services.BuildServiceProvider();
    blazorWebView.RootComponents.Add<Routes>("#app");

    window.SetChild(blazorWebView);
    window.Show();
};

app.RunWithSynchronizationContext(null);