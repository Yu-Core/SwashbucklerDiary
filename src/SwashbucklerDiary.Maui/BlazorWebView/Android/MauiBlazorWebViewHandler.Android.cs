using Android.Webkit;
using SwashbucklerDiary.Shared;
using WebView = Android.Webkit.WebView;

namespace SwashbucklerDiary.Maui.BlazorWebView
{
    public partial class MauiBlazorWebViewHandler
    {
        public static WebView? WebView { get; private set; }

#pragma warning disable CA1416 // 验证平台兼容性
        protected override void ConnectHandler(WebView platformView)
        {
            base.ConnectHandler(platformView);
            platformView.SetWebViewClient(new MauiBlazorWebViewClient(platformView.WebViewClient));
            platformView.SetWebChromeClient(new MauiBlazorWebChromeClient(platformView.WebChromeClient, this.MauiContext, platformView));
        }

#nullable disable
        private class MauiBlazorWebViewClient : WebViewClient
        {
            private readonly WebViewClient _webViewClient;

            public MauiBlazorWebViewClient(WebViewClient webViewClient)
            {
                _webViewClient = webViewClient;
            }

            public override bool ShouldOverrideUrlLoading(Android.Webkit.WebView view, IWebResourceRequest request)
            {
                return _webViewClient.ShouldOverrideUrlLoading(view, request);
            }

            public override WebResourceResponse ShouldInterceptRequest(Android.Webkit.WebView view, IWebResourceRequest request)
            {
                var intercept = InterceptCustomPathRequest(request, out WebResourceResponse webResourceResponse);
                if (intercept)
                {
                    return webResourceResponse;
                }

                return _webViewClient.ShouldInterceptRequest(view, request);
            }

            public override void OnPageFinished(Android.Webkit.WebView view, string url)
            {
                _webViewClient.OnPageFinished(view, url);

                WebView = view;
                AndroidSafeArea.Initialize(view);
            }

            protected override void Dispose(bool disposing)
            {
                if (!disposing)
                    return;

                _webViewClient.Dispose();
            }

            private static bool InterceptCustomPathRequest(IWebResourceRequest request, out WebResourceResponse webResourceResponse)
            {
                webResourceResponse = null;

                var uri = request.Url.ToString();
                if (InterceptLocalFileRequest(uri, out string filePath))
                {
                    webResourceResponse = CreateLocalFileResponse(request, filePath);
                    return true;
                }

                return false;
            }

            // 因为Android Webview 的bug，所以流只能直接返回，不能做截取
            // https://bugs.chromium.org/p/chromium/issues/detail?id=1161877#c13
            private static WebResourceResponse CreateLocalFileResponse(IWebResourceRequest request, string path)
            {
                string contentType = StaticContentProvider.GetResponseContentTypeOrDefault(path);
                var headers = StaticContentProvider.GetResponseHeaders(contentType);
                FileStream stream = File.OpenRead(path);
                var length = stream.Length;
                long rangeStart = 0;
                long rangeEnd = length - 1;

                string encoding = "UTF-8";
                int stateCode = 200;
                string reasonPhrase = "OK";

                //适用于音频视频文件资源的响应
                bool partial = request.RequestHeaders.TryGetValue("Range", out string rangeString);
                if (partial)
                {
                    //206,可断点续传
                    stateCode = 206;
                    reasonPhrase = "Partial Content";

                    ParseRange(rangeString, ref rangeStart, ref rangeEnd);
                    headers.Add("Accept-Ranges", "bytes");
                    headers.Add("Content-Range", $"bytes {rangeStart}-{rangeEnd}/{length}");
                }

                //这一行删去似乎也不影响
                headers.Add("Content-Length", (rangeEnd - rangeStart + 1).ToString());

                var response = new WebResourceResponse(contentType, encoding, stateCode, reasonPhrase, headers, stream);
                return response;
            }
        }
    }
}
