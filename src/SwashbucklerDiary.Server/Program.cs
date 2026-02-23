using Blazored.LocalStorage;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Server;
using SwashbucklerDiary.Server.Extensions;
using SwashbucklerDiary.Server.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// add support for web api
builder.Services.AddControllers();

builder.Services.AddHttpContextAccessor();

builder.Services.AddBlazoredLocalStorage();

builder.Services.AddMasaBlazorConfig();

builder.Services.AddSqlSugarConfig(SQLiteConstants.ConnectionString, SQLiteConstants.PrivacyConnectionString, ServiceLifetime.Scoped);

builder.Services.AddSerilogConfig();

builder.Services.AddDependencyInjection();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseHttpsRedirection();

app.UseMiddleware<StartPathMiddleware>();
app.UseMiddleware<ApiAuthValidationMiddleware>();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddAdditionalAssemblies([.. Routes.AdditionalAssemblies]);
// add support for web api
app.MapControllers();

app.Run();
