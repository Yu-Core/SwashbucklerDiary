using Gtk;
using Microsoft.AspNetCore.Components.WebView.Gtk;
using Microsoft.Extensions.DependencyInjection;
using SwashbucklerDiary.Gtk;
using SwashbucklerDiary.Gtk.Extensions;

#pragma warning disable CS0162 // Unreachable code detected

Application.Init();

// Create the parent window
var window = new Window(WindowType.Toplevel);
window.DefaultSize = new Gdk.Size(1024, 768);

window.DeleteEvent += (o, e) =>
{
    Application.Quit();
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

window.Add(blazorWebView);
window.ShowAll();

Application.Run();