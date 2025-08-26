using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Maui.Platform;

namespace SwashbucklerDiary.Maui
{
    public partial class MainPage
    {
        private partial void BlazorWebViewInitializing(object? sender, BlazorWebViewInitializingEventArgs e)
        {
        }

        private partial void BlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e)
        {
            e.WebView.VerticalScrollBarEnabled = false; // 关闭滚动条
            e.WebView.Settings.MediaPlaybackRequiresUserGesture = false; // 是否需要用户手势才能播放
            e.WebView.SetBackgroundColor(_backgroundColor.ToPlatform());
            e.WebView.SetWebChromeClient(new MauiBlazorWebChromeClient(e.WebView.WebChromeClient, blazorWebView.Handler?.MauiContext, e.WebView));
            e.WebView.SetWebViewClient(new MauiBlazorWebViewClient(e.WebView.WebViewClient));
        }
    }
}
