using CommunityToolkit.Maui;
using MauiBlazorToolkit.Extensions;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Maui.LifecycleEvents;
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

#if IOS || MACCATALYST
            // UIScene 生命周期下，AppDelegate.OpenUrl 不再被调用
            // 将通过 URL 访问隐私模式数据库等功能的激活逻辑转发到 SceneOpenUrl
            builder.ConfigureLifecycleEvents(events =>
            {
                events.AddiOS(ios => ios
                    .SceneOpenUrl((scene, urlContexts) =>
                    {
                        if (urlContexts is not null)
                        {
                            foreach (var context in urlContexts)
                            {
                                AppActivation.LaunchOrActivate(context.Url);
                            }
                        }

                        return true;
                    }));
            });
#endif

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

            builder.Services.AddSerilogConfig(SQLiteConstants.LogDatabasePath);

            builder.Services.AddSqlSugarConfig(SQLiteConstants.ConnectionString, SQLiteConstants.PrivacyConnectionString, SQLiteConstants.LogConnectionString);

            builder.Services.AddDependencyInjection();

            builder.Services.AddMauiExceptionHandler();

            return builder.Build();
        }
    }
}