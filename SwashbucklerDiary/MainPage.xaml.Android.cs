using Android.OS;
using Android.Webkit;
using Microsoft.AspNetCore.Components.WebView;
using SwashbucklerDiary.Utilities;

namespace SwashbucklerDiary
{
#pragma warning disable CA1416 // 验证平台兼容性
    public partial class MainPage
    {
        private partial void BlazorWebViewInitializing(object? sender, BlazorWebViewInitializingEventArgs e)
        {
        }

        private partial void BlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e)
        {
            e.WebView.VerticalScrollBarEnabled = false; // 关闭滚动条
            e.WebView.Settings.JavaScriptEnabled = true;
            e.WebView.Settings.MediaPlaybackRequiresUserGesture = false; // 是否需要用户手势才能播放
            if (Build.VERSION.SdkInt >= BuildVersionCodes.O)
            {
                e.WebView.SetWebViewClient(new MyWebViewClient(e.WebView.WebViewClient));
            }
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
                    string contentType = StaticContentProvider.GetResponseContentTypeOrDefault(path);
                    string encoding = "UTF-8";
                    Stream stream = File.OpenRead(path);
                    return new(contentType, encoding, stream);
                }

                return WebViewClient.ShouldInterceptRequest(view, request);
            }
        }
    }
}
