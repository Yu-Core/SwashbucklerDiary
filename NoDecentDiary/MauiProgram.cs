using Microsoft.AspNetCore.Components.WebView.Maui;
using NoDecentDiary.Extend;
using NoDecentDiary.StaticData;
using Serilog;

namespace NoDecentDiary;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
            });

        builder.Services.AddMauiBlazorWebView();
#if DEBUG
        builder.Services.AddBlazorWebViewDeveloperTools();
#endif

        #region Log
        if (!Directory.Exists(SerilogConstants.folderPath))
        {
            Directory.CreateDirectory(SerilogConstants.folderPath);
        }

        Log.Logger = new LoggerConfiguration()
             .Enrich.FromLogContext()
             .WriteTo.Debug()
             .WriteTo.File(path: SerilogConstants.filePath)
             .CreateLogger();
        #endregion

        builder.Services.AddLogging(logging =>
            {
                logging.AddSerilog(dispose: true);
            });
        builder.Services.AddCustomIOC();
        builder.Services.AddMasaBlazor();

        return builder.Build();
    }
}
