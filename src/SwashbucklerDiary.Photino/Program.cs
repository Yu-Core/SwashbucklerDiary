using Microsoft.Extensions.DependencyInjection;
using Photino.Blazor;
using SwashbucklerDiary.Photino.Extensions;
using SwashbucklerDiary.Photino.Services;
using SwashbucklerDiary.Rcl;
using SwashbucklerDiary.Shared;

internal class Program
{
    [STAThread]
    private static void Main(string[] args)
    {
        var appBuilder = PhotinoBlazorAppBuilder.CreateDefault(args);

        appBuilder.RootComponents.Add<App>("#app");
        appBuilder.Services.AddMasaBlazor();
        appBuilder.Services.AddDependencyInjection();

        var app = appBuilder.Build();

        app.MainWindow
            .SetTitle("Photino Blazor Sample");

        AppDomain.CurrentDomain.UnhandledException += (sender, error) =>
        {
        };

        app.Run();
    }
}