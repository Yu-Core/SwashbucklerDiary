using CommunityToolkit.Maui.Core;
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
                .UseMauiCommunityToolkitCore()
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

            builder.Services.AddSerilogConfig();
            builder.Services.AddSqlsugarConfig();
            builder.Services.AddMasaBlazorConfig();
            builder.Services.AddCustomIOC();

            return builder.Build();
        }
    }
}
