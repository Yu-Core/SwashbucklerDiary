using Android.Webkit;

namespace SwashbucklerDiary.Maui
{
#nullable disable
    public class MauiBlazorWebViewClient : WebViewClient
    {
        private readonly WebViewClient _webViewClient;
        private AndroidSafeArea _androidSafeArea;

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
            return _webViewClient.ShouldInterceptRequest(view, request);
        }

        public override void OnPageFinished(Android.Webkit.WebView view, string url)
        {
            _webViewClient.OnPageFinished(view, url);
            _androidSafeArea = new AndroidSafeArea(view);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            _webViewClient.Dispose();
            _androidSafeArea.Dispose();
        }
    }
}
