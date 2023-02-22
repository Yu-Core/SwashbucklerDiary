using SwashbucklerDiary.Extend;

namespace SwashbucklerDiary
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
                })
                .ConfigureEssentials(essentials =>
                {
                    essentials.UseVersionTracking();
                });

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif

            builder.Services.AddSerilog();
            builder.Services.AddSqlsugar();
            builder.Services.AddMasaBlazor().AddI18nForMauiBlazor("wwwroot/i18n");
            builder.Services.AddCustomIOC();

            return builder.Build();
        }
    }
}
