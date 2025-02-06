using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.WebAssembly;
using SwashbucklerDiary.WebAssembly.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddSingleton(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazoredLocalStorageAsSingleton();

await builder.Services.AddFileSystem();

await builder.Services.AddMasaBlazorConfig(builder.HostEnvironment.BaseAddress);

builder.Services.AddSqlSugarConfig($"Data Source={SQLiteConstants.DatabasePath}", $"Data Source={SQLiteConstants.PrivacyDatabasePath}");

builder.Services.AddSerilogConfig();

builder.Services.AddDependencyInjection();

await builder.Build().RunAsync();
