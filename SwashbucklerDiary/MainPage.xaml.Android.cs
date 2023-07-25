using Android.Webkit;
using Microsoft.AspNetCore.Components.WebView;

namespace SwashbucklerDiary
{
    public partial class MainPage
    {
        private partial void BlazorWebViewInitializing(object? sender, BlazorWebViewInitializingEventArgs e)
        {
        }

        private partial void BlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e)
        {
            e.WebView.VerticalScrollBarEnabled = false; // 关闭滚动条
            e.WebView.Settings.JavaScriptEnabled = true;
            e.WebView.SetWebViewClient(new MyWebViewClient(e.WebView.WebViewClient));
        }

#nullable disable
        private class MyWebViewClient : WebViewClient
        {
            private WebViewClient WebViewClient { get; }

            public MyWebViewClient(WebViewClient webViewClient)
            {
                WebViewClient = webViewClient;
            }

            public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView view, IWebResourceRequest request)
            {
                return WebViewClient.ShouldOverrideUrlLoading(view, request);
            }

            public override WebResourceResponse ShouldInterceptRequest(Android.Webkit.WebView view, IWebResourceRequest request)
            {
                if (request.Url.Scheme == "appdata")
                {
                    return InterceptAppDataRequest(view, request);
                }

                return WebViewClient.ShouldInterceptRequest(view, request);
            }

            public override void OnPageFinished(Android.Webkit.WebView view, string url)
            => WebViewClient.OnPageFinished(view, url);

            protected override void Dispose(bool disposing)
            {
                if (!disposing)
                    return;

                WebViewClient.Dispose();
            }

            private WebResourceResponse InterceptAppDataRequest(Android.Webkit.WebView view, IWebResourceRequest request)
            {
                var path = request.Url.Path;
                path = FileSystem.AppDataDirectory + path;
                if (File.Exists(path))
                {
                    string mime = MimeTypeMap.Singleton.GetMimeTypeFromExtension(Path.GetExtension(path));
                    string encoding = "UTF-8";
                    Stream stream = File.OpenRead(path);
                    return new(mime, encoding, stream);
                }

                return WebViewClient.ShouldInterceptRequest(view, request);
            }
        }
    }
}
