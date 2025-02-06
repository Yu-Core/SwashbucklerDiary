using CommunityToolkit.Maui;
using MauiBlazorToolkit.Extensions;
using Microsoft.AspNetCore.Components.WebView.Maui;
using SwashbucklerDiary.Maui.BlazorWebView;
using SwashbucklerDiary.Maui.Essentials;
using SwashbucklerDiary.Maui.Extensions;
using SwashbucklerDiary.Rcl.Extensions;

namespace SwashbucklerDiary.Maui
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiBlazorToolkit(options =>
                {
                    options.TitleBar = true;
                })
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                })
                .ConfigureEssentials(essentials =>
                {
                    essentials.UseVersionTracking()
#if !WINDOWS
                              .OnAppAction(AppActionsHelper.HandleAppActions)
#endif
                              ;
                });

            builder.Services.AddMauiBlazorWebView();

            builder.Services.ConfigureMauiHandlers(delegate (IMauiHandlersCollection handlers)
            {
                handlers.AddHandler<IBlazorWebView>((IServiceProvider _) => new MauiBlazorWebViewHandler());
            });

#if IOS || MACCATALYST
            BlazorWebViewHandler.BlazorWebViewMapper.AppendToMapping(nameof(IBlazorWebView.HostPage), (handler, view) =>
            {
                handler.PlatformView.NavigationDelegate = new WebViewNavigationDelegate();
            });
            BlazorWebViewHandler.BlazorWebViewMapper.AppendToMapping(nameof(IBlazorWebView.RootComponents), (handler, view) =>
            {
                handler.PlatformView.NavigationDelegate = new WebViewNavigationDelegate();
            });
#endif

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif
            builder.Services.AddMasaBlazorConfig();

            builder.Services.AddSerilogConfig();

            builder.Services.AddSqlSugarConfig(SQLiteConstants.ConnectionString, SQLiteConstants.PrivacyConnectionString);

            builder.Services.AddDependencyInjection();

            builder.Services.AddMauiExceptionHandler();

            return builder.Build();
        }
    }
}