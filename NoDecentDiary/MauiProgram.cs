using Microsoft.AspNetCore.Components.WebView.Maui;
using NoDecentDiary.Extend;
using NoDecentDiary.StaticData;
using Serilog;
using Serilog.Events;

namespace NoDecentDiary
{

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
#if DEBUG
                            .MinimumLevel.Debug()
#else
                            .MinimumLevel.Information()
#endif
                            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
                 .Enrich.FromLogContext()
                 .WriteTo.Async(c => c.File(SerilogConstants.filePath,rollingInterval:RollingInterval.Day))
                 .CreateLogger();
            #endregion

            builder.Services.AddLogging(logging =>
                {
                    logging.AddSerilog(dispose: true);
                });
            builder.Services.AddCustomIOC();
            builder.Services.AddMasaBlazor();
            builder.Services.AddMasaBlazor().AddI18nForMauiBlazor("i18n");

            return builder.Build();
        }
    }
}
