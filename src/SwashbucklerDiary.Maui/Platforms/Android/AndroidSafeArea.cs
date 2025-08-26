using Android.App;
using Android.Views;
using Android.Widget;
using SwashbucklerDiary.Maui.Extensions;
using static Android.Resource;
using Insets = AndroidX.Core.Graphics.Insets;
using View = Android.Views.View;
using WebView = Android.Webkit.WebView;

namespace SwashbucklerDiary.Maui
{
#nullable disable
    public sealed class AndroidSafeArea : IDisposable
    {
        private readonly WeakReference<Activity> _activityRef;
        private readonly WeakReference<WebView> _webViewRef;
        private ViewTreeObserver _viewTreeObserver;
        private bool _disposed;

        public AndroidSafeArea(WebView webView)
        {
            if (webView?.Context is not Activity activity)
                throw new ArgumentNullException(nameof(webView));

            _webViewRef = new WeakReference<WebView>(webView);
            _activityRef = new WeakReference<Activity>(activity);

            FrameLayout content = activity.FindViewById<FrameLayout>(Id.Content);
            View rootView = content?.GetChildAt(0);

            if (rootView is not null)
            {
                _viewTreeObserver = rootView.ViewTreeObserver;
                _viewTreeObserver.GlobalLayout += HandleGlobalLayout;
            }

            HandleGlobalLayout(); // Initial setup
        }

        private void HandleGlobalLayout(object sender = null, EventArgs e = null)
        {
            if (!_activityRef.TryGetTarget(out Activity activity))
                return;

            Insets statusBarInsets = activity.GetStatusBarInsets();
            Insets navigationBarInsets = activity.GetNavigationBarInsets();

            // DeviceDisplay.Current.MainDisplayInfo.Orientation is inaccurate first entry, It must be Portrait
            bool isLandscape = DeviceDisplay.Current.MainDisplayInfo.Width > DeviceDisplay.Current.MainDisplayInfo.Height;
            int safeAreaInsetBottom = isLandscape ? 0 : navigationBarInsets.Bottom;

            SetSafeAreaCss(
                statusBarInsets.Top,
                safeAreaInsetBottom,
                navigationBarInsets.Left,
                navigationBarInsets.Right
            );
        }

        private void SetSafeAreaCss(int top, int bottom, int left, int right)
        {
            if (!_activityRef.TryGetTarget(out Activity activity))
                return;

            if (!_webViewRef.TryGetTarget(out WebView webView))
                return;

            string js = $@"
                document.documentElement.style.setProperty('--safe-area-inset-top','{activity.PxToDip(top)}px');
                document.documentElement.style.setProperty('--safe-area-inset-bottom','{activity.PxToDip(bottom)}px');                
                document.documentElement.style.setProperty('--safe-area-inset-left','{activity.PxToDip(left)}px');
                document.documentElement.style.setProperty('--safe-area-inset-right','{activity.PxToDip(right)}px');
            ";

            webView.EvaluateJavascript(js, null);
        }

        public void Dispose()
        {
            if (_disposed)
                return;

            if (_viewTreeObserver is not null)
            {
                _viewTreeObserver.GlobalLayout -= HandleGlobalLayout;
            }

            _viewTreeObserver = null;
            _disposed = true;
        }
    }
}