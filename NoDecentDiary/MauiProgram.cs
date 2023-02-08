using NoDecentDiary.Extend;

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
                })
                .ConfigureEssentials(essentials =>
                {
                    essentials.UseVersionTracking();
                });

            builder.Services.AddMauiBlazorWebView();
#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif

            builder.Services.AddLog();
            builder.Services.AddCustomIOC();
            builder.Services.AddMasaBlazor();
            builder.Services.AddMasaBlazor().AddI18nForMauiBlazor("wwwroot/i18n");

            return builder.Build();
        }
    }
}
