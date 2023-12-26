using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Maui.Platform;

namespace SwashbucklerDiary.Maui
{
    public partial class MainPage
    {
        private partial void BlazorWebViewInitializing(object? sender, BlazorWebViewInitializingEventArgs e)
        {
            e.EnvironmentOptions = new()
            {
                //禁用自动播放，但不知道为什么没有生效
                AdditionalBrowserArguments = "--autoplay-policy=user-gesture-required",
            };
        }

        private partial void BlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e)
        {
            //https://learn.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2controller.zoomfactor?view=webview2-dotnet-1.0.1370.28
            //e.WebView.DefaultBackgroundColor = Color.FromArgb("#f7f8f9").ToWindowsColor();
        }
    }
}
