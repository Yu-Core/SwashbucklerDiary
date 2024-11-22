using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Webkit;
using Android.Widget;
using AndroidX.Core.View;
using Java.Interop;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Handlers;
using Microsoft.Maui.Platform;
using SwashbucklerDiary.Rcl.Essentials;
using View = Android.Views.View;
using WebView = Android.Webkit.WebView;

namespace SwashbucklerDiary.Maui.BlazorWebView;

#nullable disable
internal class MauiBlazorWebChromeClient : WebChromeClient
{
    private readonly WebChromeClient _blazorWebChromeClient;
    private WeakReference<Activity> _activityRef;
    private readonly WebView _webView;

    View _customView;
    ICustomViewCallback _videoViewCallback;
    int _defaultSystemUiVisibility;
    bool _isSystemBarVisible;
    Android.Content.PM.ScreenOrientation screenOrientation;

    private readonly Lazy<INavigateController> _navigateController = new(() => IPlatformApplication.Current!.Services.GetRequiredService<INavigateController>());

    private INavigateController NavigateController => _navigateController.Value;

    public MauiBlazorWebChromeClient(WebChromeClient blazorWebChromeClient, IMauiContext mauiContext, WebView webView)
    {
        _blazorWebChromeClient = blazorWebChromeClient;
        SetContext(mauiContext);
        _webView = webView;
    }

    // OnShowCustomView operate the perform call back to video view functionality
    // is visible in the view.
    public override void OnShowCustomView(View view, ICustomViewCallback callback)
    {
        if (_customView is not null)
        {
            OnHideCustomView();
            return;
        }

        _activityRef.TryGetTarget(out Activity context);

        if (context is null)
            return;

        NavigateController.AddHistoryAction(OnHideCustomView);

        _videoViewCallback = callback;
        _customView = view;
        _customView.SetBackgroundColor(Android.Graphics.Color.White);

        screenOrientation = context.RequestedOrientation;

        // Add the CustomView
        if (context.Window.DecorView is FrameLayout layout)
            layout.AddView(_customView, new FrameLayout.LayoutParams(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent));

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            await Task.Delay(200);
            _webView.EvaluateJavascript(@"
				(function() {
                    const fullscreenElement = document.fullscreenElement;
                    if (fullscreenElement && fullscreenElement.nodeName === 'VIDEO') {
                        return fullscreenElement.videoWidth > fullscreenElement.videoHeight;
                    }
                    return true;
                })();
			", new JavaScriptValueCallback(async result =>
            {
                bool isLandscape = Convert.ToBoolean(result.ToString());
                if (isLandscape)
                {
                    context.RequestedOrientation = Android.Content.PM.ScreenOrientation.Landscape;
                }

                await Task.Delay(200);

                // Hide the SystemBars and Status bar
                if (OperatingSystem.IsAndroidVersionAtLeast(30))
                {
                    if (!OperatingSystem.IsAndroidVersionAtLeast(35))
                    {
                        context.Window.SetDecorFitsSystemWindows(false);
                    }

                    var windowInsets = context.Window.DecorView.RootWindowInsets;
                    _isSystemBarVisible = windowInsets.IsVisible(WindowInsetsCompat.Type.NavigationBars()) || windowInsets.IsVisible(WindowInsetsCompat.Type.StatusBars());

                    if (_isSystemBarVisible)
                        context.Window.InsetsController?.Hide(WindowInsets.Type.SystemBars());
                }
                else
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    _defaultSystemUiVisibility = (int)context.Window.DecorView.SystemUiVisibility;
                    int systemUiVisibility = _defaultSystemUiVisibility | (int)SystemUiFlags.LayoutStable | (int)SystemUiFlags.LayoutHideNavigation | (int)SystemUiFlags.LayoutHideNavigation |
                        (int)SystemUiFlags.LayoutFullscreen | (int)SystemUiFlags.HideNavigation | (int)SystemUiFlags.Fullscreen | (int)SystemUiFlags.Immersive;
                    context.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)systemUiVisibility;
#pragma warning restore CS0618 // Type or member is obsolete
                }
            }));
        });
    }

    // OnHideCustomView is the WebView call back when the load view in full screen
    // and hide the custom container view.
    public override void OnHideCustomView()
    {
        _activityRef.TryGetTarget(out Activity context);

        if (context is null)
            return;

        NavigateController.RemoveHistoryAction(OnHideCustomView);

        // Remove the CustomView
        if (context.Window.DecorView is FrameLayout layout)
            layout.RemoveView(_customView);

        context.RequestedOrientation = screenOrientation;

        _videoViewCallback.OnCustomViewHidden();
        _customView = null;
        _videoViewCallback = null;
        // When in landscape mode, If this is not added, the soft keyboard will not display
        context.CurrentFocus?.ClearFocus();

        // Show again the SystemBars and Status bar
        if (OperatingSystem.IsAndroidVersionAtLeast(30))
        {
            if (!OperatingSystem.IsAndroidVersionAtLeast(35))
            {
                context.Window.SetDecorFitsSystemWindows(true);
            }

            if (_isSystemBarVisible)
                context.Window.InsetsController?.Show(WindowInsets.Type.SystemBars());
        }
        else
#pragma warning disable CS0618 // Type or member is obsolete
            context.Window.DecorView.SystemUiVisibility = (StatusBarVisibility)_defaultSystemUiVisibility;
#pragma warning restore CS0618 // Type or member is obsolete
    }

    // See: https://github.com/dotnet/maui/issues/6565
    public override JniPeerMembers JniPeerMembers => _blazorWebChromeClient.JniPeerMembers;
    public override Bitmap DefaultVideoPoster => _blazorWebChromeClient.DefaultVideoPoster;
    public override View VideoLoadingProgressView => _blazorWebChromeClient.VideoLoadingProgressView;
    public override void GetVisitedHistory(IValueCallback callback)
        => _blazorWebChromeClient.GetVisitedHistory(callback);
    public override bool OnConsoleMessage(ConsoleMessage consoleMessage)
        => _blazorWebChromeClient.OnConsoleMessage(consoleMessage);
    public override bool OnCreateWindow(WebView view, bool isDialog, bool isUserGesture, Message resultMsg)
        => _blazorWebChromeClient.OnCreateWindow(view, isDialog, isUserGesture, resultMsg);
    public override void OnGeolocationPermissionsHidePrompt()
        => _blazorWebChromeClient.OnGeolocationPermissionsHidePrompt();
    public override bool OnJsAlert(WebView view, string url, string message, JsResult result)
        => _blazorWebChromeClient.OnJsAlert(view, url, message, result);
    public override bool OnJsBeforeUnload(WebView view, string url, string message, JsResult result)
        => _blazorWebChromeClient.OnJsBeforeUnload(view, url, message, result);
    public override bool OnJsConfirm(WebView view, string url, string message, JsResult result)
        => _blazorWebChromeClient.OnJsConfirm(view, url, message, result);
    public override bool OnJsPrompt(WebView view, string url, string message, string defaultValue, JsPromptResult result)
        => _blazorWebChromeClient.OnJsPrompt(view, url, message, defaultValue, result);
    public override void OnPermissionRequestCanceled(PermissionRequest request)
        => _blazorWebChromeClient.OnPermissionRequestCanceled(request);
    public override void OnProgressChanged(WebView view, int newProgress)
        => _blazorWebChromeClient.OnProgressChanged(view, newProgress);
    public override void OnReceivedIcon(WebView view, Bitmap icon)
        => _blazorWebChromeClient.OnReceivedIcon(view, icon);
    public override void OnReceivedTitle(WebView view, string title)
        => _blazorWebChromeClient.OnReceivedTitle(view, title);
    public override void OnReceivedTouchIconUrl(WebView view, string url, bool precomposed)
        => _blazorWebChromeClient.OnReceivedTouchIconUrl(view, url, precomposed);
    public override void OnRequestFocus(WebView view)
        => _blazorWebChromeClient.OnRequestFocus(view);
    public override bool OnShowFileChooser(WebView webView, IValueCallback filePathCallback, FileChooserParams fileChooserParams)
        => _blazorWebChromeClient.OnShowFileChooser(webView, filePathCallback, fileChooserParams);
    public override void OnCloseWindow(WebView window)
        => _blazorWebChromeClient.OnCloseWindow(window);
    public override void OnGeolocationPermissionsShowPrompt(string origin, GeolocationPermissions.ICallback callback)
        => _blazorWebChromeClient.OnGeolocationPermissionsShowPrompt(origin, callback);
    public override void OnPermissionRequest(PermissionRequest request)
        => _blazorWebChromeClient.OnPermissionRequest(request);

    void SetContext(IMauiContext mauiContext)
    {
        var activity = (mauiContext?.Context?.GetActivity()) ?? Platform.CurrentActivity;

        if (activity is null)
            mauiContext?.Services.GetService<ILogger<WebViewHandler>>()?.LogWarning($"Failed to set the activity of the WebChromeClient, can't show pickers on the Webview");

        _activityRef = new WeakReference<Activity>(activity);
    }

    private class JavaScriptValueCallback : Java.Lang.Object, IValueCallback
    {
        private readonly Action<Java.Lang.Object> _callback;

        public JavaScriptValueCallback(Action<Java.Lang.Object> callback)
        {
            ArgumentNullException.ThrowIfNull(callback);
            _callback = callback;
        }

        public void OnReceiveValue(Java.Lang.Object value)
        {
            _callback(value);
        }
    }
}