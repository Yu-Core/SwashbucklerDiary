using Android.Webkit;
using SwashbucklerDiary.Shared;
using WebView = Android.Webkit.WebView;

namespace SwashbucklerDiary.Maui.BlazorWebView
{
    public partial class MauiBlazorWebViewHandler
    {
#pragma warning disable CA1416 // 验证平台兼容性
        protected override void ConnectHandler(WebView platformView)
        {
            base.ConnectHandler(platformView);
            platformView.SetWebViewClient(new MyWebViewClient(platformView.WebViewClient));
            platformView.SetWebChromeClient(new MauiBlazorWebChromeClient(platformView.WebChromeClient, this.MauiContext, platformView));
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
                var intercept = InterceptCustomPathRequest(request, out WebResourceResponse webResourceResponse);
                if (intercept)
                {
                    return webResourceResponse;
                }

                return WebViewClient.ShouldInterceptRequest(view, request);
            }

            public override void OnPageFinished(Android.Webkit.WebView view, string url)
            {
                WebViewClient.OnPageFinished(view, url);

                AndroidSafeArea.Initialize(view);
            }

            protected override void Dispose(bool disposing)
            {
                if (!disposing)
                    return;

                WebViewClient.Dispose();
            }

            private static bool InterceptCustomPathRequest(IWebResourceRequest request, out WebResourceResponse webResourceResponse)
            {
                webResourceResponse = null;

                var uri = request.Url.ToString();
                if (!Intercept(uri, out string path))
                {
                    return false;
                }

                if (!File.Exists(path))
                {
                    return false;
                }

                webResourceResponse = CreateWebResourceResponse(request, path);
                return true;
            }

            //因为Android Webview 的一个bug，所以流只能直接返回，不能做截取
            //https://bugs.chromium.org/p/chromium/issues/detail?id=1161877#c13
            private static WebResourceResponse CreateWebResourceResponse(IWebResourceRequest request, string path)
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

                    var ranges = rangeString.Split('=');
                    if (ranges.Length > 1 && !string.IsNullOrEmpty(ranges[1]))
                    {
                        string[] rangeDatas = ranges[1].Split("-");
                        rangeStart = Convert.ToInt64(rangeDatas[0]);
                        if (rangeDatas.Length > 1 && !string.IsNullOrEmpty(rangeDatas[1]))
                        {
                            rangeEnd = Convert.ToInt64(rangeDatas[1]);
                        }
                    }

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
