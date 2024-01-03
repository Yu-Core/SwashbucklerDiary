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
                //语言设置也不生效，疑似与WinUI3中Environment不生效有关
                Language = "zh-CN"
            };
        }

        private partial void BlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e)
        {
            //https://learn.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2controller.defaultbackgroundcolor?view=webview2-dotnet-1.0.1370.28

            e.WebView.DefaultBackgroundColor = color.ToWindowsColor();
        }
    }
}
