using Microsoft.Extensions.DependencyInjection;
using SwashbucklerDiary.Gtk;
using SwashbucklerDiary.Gtk.Extensions;

var services = new ServiceCollection();
services.AddGtkBlazorWebView();
#if DEBUG
services.AddBlazorWebViewDeveloperTools();
#endif

services.AddMasaBlazorConfig();

services.AddSerilogConfig();

services.AddSqlsugarConfig();

services.AddDependencyInjection();

var sp = services.BuildServiceProvider();

var app = new App(sp);
app.Run();