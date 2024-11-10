using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Maui.Platform;
using Microsoft.UI.Windowing;
using Microsoft.Web.WebView2.Core;

namespace SwashbucklerDiary.Maui
{
    public partial class MainPage
    {
        private static AppWindow? AppWindow => (Application.Current?.Windows[0]?.Handler?.PlatformView as Microsoft.UI.Xaml.Window)?.AppWindow;

        private partial void BlazorWebViewInitializing(object? sender, BlazorWebViewInitializingEventArgs e)
        {
        }

        private partial void BlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e)
        {
            //https://learn.microsoft.com/en-us/dotnet/api/microsoft.web.webview2.core.corewebview2controller.defaultbackgroundcolor?view=webview2-dotnet-1.0.1370.28

            e.WebView.DefaultBackgroundColor = _backgroundColor.ToWindowsColor();
            e.WebView.CoreWebView2.ContainsFullScreenElementChanged += FullScreen;
            e.WebView.AllowDrop = true;
            e.WebView.CoreWebView2.Settings.UserAgent += " Android Mobile";
        }

        private void FullScreen(CoreWebView2 coreWebView2, object args)
        {
            if (coreWebView2.ContainsFullScreenElement)
            {
                AppWindow?.SetPresenter(AppWindowPresenterKind.FullScreen);
            }
            else
            {
                AppWindow?.SetPresenter(AppWindowPresenterKind.Default);
            }
        }
    }
}
