using Android.OS;
using Android.Webkit;
using Microsoft.AspNetCore.Components.WebView;
using SwashbucklerDiary.Extensions;
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
            //e.WebView.Settings.MediaPlaybackRequiresUserGesture = false; // 是否需要用户手势才能播放
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
                var intercept = InterceptCustomPathRequest(request, out WebResourceResponse webResourceResponse);
                if (intercept)
                {
                    return webResourceResponse;
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

            private static bool InterceptCustomPathRequest(IWebResourceRequest request, out WebResourceResponse webResourceResponse)
            {
                webResourceResponse = null;

                var uri = request.Url.ToString();
                if (!uri.StartsWith(StaticCustomPath.InterceptPrefix))
                {
                    return false;
                }

                var path = uri.TrimStart(StaticCustomPath.InterceptPrefix);
                path = Path.Combine(FileSystem.AppDataDirectory, path);
                if (!File.Exists(path))
                {
                    return false;
                }

                webResourceResponse = CreateWebResourceResponse(request, path);
                return true;
            }

            private static WebResourceResponse CreateWebResourceResponse(IWebResourceRequest request, string path)
            {
                string contentType = StaticContentProvider.GetResponseContentTypeOrDefault(path);
                var headers = StaticContentProvider.GetResponseHeaders(contentType);
                Stream stream = File.OpenRead(path);
                string encoding = "UTF-8";
                int stateCode = 200;

                var length = stream.Length;
                long rangeStart = 0;
                long rangeEnd = length - 1;

                bool partial = request.RequestHeaders.TryGetValue("Range", out string rangeString);
                if (partial)
                {
                    //206,可断点续传
                    stateCode = 206;

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

                //不是必要的，删去似乎也不影响
                headers.Add("Content-Length", (rangeEnd - rangeStart + 1).ToString());

                var response = new WebResourceResponse(contentType, encoding, stateCode, "OK", headers, stream);
                return response;
            }

        }
    }
}
