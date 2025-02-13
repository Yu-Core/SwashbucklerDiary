using Microsoft.Extensions.DependencyInjection;
using SwashbucklerDiary.Gtk;
using SwashbucklerDiary.Gtk.Extensions;
using SwashbucklerDiary.Rcl.Extensions;

var services = new ServiceCollection();
services.AddGtkBlazorWebView();
#if DEBUG
services.AddBlazorWebViewDeveloperTools();
#endif

services.AddMasaBlazorConfig();

services.AddSerilogConfig();

services.AddSqlSugarConfig(SQLiteConstants.ConnectionString, SQLiteConstants.PrivacyConnectionString);

services.AddDependencyInjection();

var sp = services.BuildServiceProvider();

var app = new App(sp);
return app.Run(args);