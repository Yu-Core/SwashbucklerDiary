using Foundation;
using Microsoft.AspNetCore.Components.WebView;
using Microsoft.AspNetCore.Components.WebView.Maui;
using UIKit;
using WebKit;

namespace SwashbucklerDiary.Maui.BlazorWebView
{
    // Fix iframe unable to open in webview
    internal class WebViewNavigationDelegate : WKNavigationDelegate
    {
        private readonly BlazorWebViewHandler _webView;

        private WKNavigation? _currentNavigation;
        private Uri? _currentUri;

        public WebViewNavigationDelegate(BlazorWebViewHandler webView)
        {
            _webView = webView ?? throw new ArgumentNullException(nameof(webView));
        }

        public override void DidStartProvisionalNavigation(WKWebView webView, WKNavigation navigation)
        {
            _currentNavigation = navigation;
        }

        public override void DecidePolicy(WKWebView webView, WKNavigationAction navigationAction, Action<WKNavigationActionPolicy> decisionHandler)
        {
            var requestUrl = navigationAction.Request.Url;
            var uri = new Uri(requestUrl.ToString());

            UrlLoadingStrategy strategy;

            // TargetFrame is null for navigation to a new window (`_blank`)
            if (navigationAction.TargetFrame is null)
            {
                // Open in a new browser window regardless of UrlLoadingStrategy
                strategy = UrlLoadingStrategy.OpenExternally;
            }
            else
            {
                // Invoke the UrlLoading event to allow overriding the default link handling behavior
                //var callbackArgs = UrlLoadingEventArgs.CreateWithDefaultLoadingStrategy(uri, BlazorWebViewHandler.AppOriginUri);
                //_webView.UrlLoading(callbackArgs);
                //_webView.Logger.NavigationEvent(uri, callbackArgs.UrlLoadingStrategy);

                //strategy = callbackArgs.UrlLoadingStrategy;
                if (navigationAction.NavigationType == WKNavigationType.Other)
                {
                    strategy = UrlLoadingStrategy.OpenInWebView;
                }
                else
                {
                    strategy = new Uri(MauiBlazorWebViewHandler.BaseUri).IsBaseOf(uri) ?
                        UrlLoadingStrategy.OpenInWebView :
                        UrlLoadingStrategy.OpenExternally;
                }
            }

            if (strategy == UrlLoadingStrategy.OpenExternally)
            {
                //_webView.Logger.LaunchExternalBrowser(uri);

#pragma warning disable CA1416, CA1422 // TODO: OpenUrl(...) has [UnsupportedOSPlatform("ios10.0")]
                UIApplication.SharedApplication.OpenUrl(requestUrl);
#pragma warning restore CA1416, CA1422
            }

            if (strategy != UrlLoadingStrategy.OpenInWebView)
            {
                // Cancel any further navigation as we've either opened the link in the external browser
                // or canceled the underlying navigation action.
                decisionHandler(WKNavigationActionPolicy.Cancel);
                return;
            }

            if (navigationAction.TargetFrame!.MainFrame)
            {
                _currentUri = requestUrl;
            }

            decisionHandler(WKNavigationActionPolicy.Allow);
        }

        public override void DidReceiveServerRedirectForProvisionalNavigation(WKWebView webView, WKNavigation navigation)
        {
            // We need to intercept the redirects to the app scheme because Safari will block them.
            // We will handle these redirects through the Navigation Manager.
            if (_currentUri?.Host == MauiBlazorWebViewHandler.AppHostAddress)
            {
                var uri = _currentUri;
                _currentUri = null;
                _currentNavigation = null;
                if (uri is not null)
                {
                    var request = new NSUrlRequest(new NSUrl(uri.AbsoluteUri));
                    webView.LoadRequest(request);
                }
            }
        }

        public override void DidFailNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            _currentUri = null;
            _currentNavigation = null;
        }

        public override void DidFailProvisionalNavigation(WKWebView webView, WKNavigation navigation, NSError error)
        {
            _currentUri = null;
            _currentNavigation = null;
        }

        public override void DidCommitNavigation(WKWebView webView, WKNavigation navigation)
        {
            if (_currentUri != null && _currentNavigation == navigation)
            {
                // TODO: Determine whether this is needed
                //_webView.HandleNavigationStarting(_currentUri);
            }
        }

        public override void DidFinishNavigation(WKWebView webView, WKNavigation navigation)
        {
            if (_currentUri != null && _currentNavigation == navigation)
            {
                // TODO: Determine whether this is needed
                //_webView.HandleNavigationFinished(_currentUri);
                _currentUri = null;
                _currentNavigation = null;
            }
        }
    }
}
