using Foundation;
using Microsoft.AspNetCore.Components.WebView;
using System.Runtime.Versioning;
using WebKit;

namespace SwashbucklerDiary
{
    public partial class MainPage
    {
        private partial void BlazorWebViewInitializing(object? sender, BlazorWebViewInitializingEventArgs e)
        {
            e.Configuration.AllowsInlineMediaPlayback = true;
            e.Configuration.MediaTypesRequiringUserActionForPlayback = WebKit.WKAudiovisualMediaTypes.None;
            e.Configuration.SetUrlSchemeHandler(new AppDataSchemeHandler(), "appdata");
        }

        private partial void BlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e)
        {
            e.WebView.ScrollView.ShowsVerticalScrollIndicator = false; // 关闭滚动条
        }

        private class AppDataSchemeHandler : NSObject, IWKUrlSchemeHandler
        {
            [Export("webView:startURLSchemeTask:")]
            [SupportedOSPlatform("ios11.0")]
            public void StartUrlSchemeTask(WKWebView webView, IWKUrlSchemeTask urlSchemeTask)
            {
                if (urlSchemeTask.Request.Url == null)
                {
                    return;
                }

                var path = urlSchemeTask.Request.Url?.Path ?? "";
                path = FileSystem.AppDataDirectory + path;
                if (File.Exists(path))
                {
                    byte[] bytes = File.ReadAllBytes(path);
                    using var response = new NSHttpUrlResponse(urlSchemeTask.Request.Url, 200, "HTTP/1.1", null);
                    urlSchemeTask.DidReceiveResponse(response);
                    urlSchemeTask.DidReceiveData(NSData.FromArray(bytes));
                    urlSchemeTask.DidFinish();
                }
            }

            [Export("webView:stopURLSchemeTask:")]
            public void StopUrlSchemeTask(WKWebView webView, IWKUrlSchemeTask urlSchemeTask)
            {
            }
        }
    }
}
