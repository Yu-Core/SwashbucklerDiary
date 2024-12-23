﻿using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using SwashbucklerDiary.WebAssembly;
using SwashbucklerDiary.WebAssembly.Extensions;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

builder.Services.AddBlazoredLocalStorage();

await builder.Services.AddFileSystem();

await builder.Services.AddMasaBlazorConfig(builder.HostEnvironment.BaseAddress);

builder.Services.AddSqlsugarConfig();

builder.Services.AddSerilogConfig();

builder.Services.AddDependencyInjection();

await builder.Build().RunAsync();
