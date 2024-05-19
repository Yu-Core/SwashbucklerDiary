using Android.Widget;
using static Android.Resource;

namespace SwashbucklerDiary.Maui
{
#nullable disable
    public static class AndroidSafeArea
    {
        private static bool initialized;

        public static void Initialize(Android.Webkit.WebView webView)
        {
            if (initialized)
            {
                return;
            }

            initialized = true;
            SetSafeAreaCss(webView);
            FrameLayout content = (FrameLayout)Platform.CurrentActivity.FindViewById(Id.Content);
            content.GetChildAt(0).ViewTreeObserver.GlobalLayout += (s, o) => SetSafeAreaCss(webView);
        }

        private static void SetSafeAreaCss(Android.Webkit.WebView webView)
        {
            int safeAreaInsetTop = Utilities.GetStatusBarInsets().Top;
            AndroidX.Core.Graphics.Insets navigationBarInsets = Utilities.GetNavigationBarInsets();
            int safeAreaInsetBottom;
            // DeviceDisplay.Current.MainDisplayInfo.Orientation is inaccurate first entry, It must be Portrait
            if (DeviceDisplay.Current.MainDisplayInfo.Width > DeviceDisplay.Current.MainDisplayInfo.Height)
            {
                safeAreaInsetBottom = 0;
            }
            else
            {
                safeAreaInsetBottom = navigationBarInsets.Bottom;
            }

            int safeAreaInsetLeft = navigationBarInsets.Left;
            int safeAreaInsetRight = navigationBarInsets.Right;
            SetSafeAreaCss(webView, safeAreaInsetTop, safeAreaInsetBottom, safeAreaInsetLeft, safeAreaInsetRight);
        }

        private static void SetSafeAreaCss(Android.Webkit.WebView webView, int safeAreaInsetTop, int safeAreaInsetBottom, int safeAreaInsetLeft, int safeAreaInsetRight)
        {
            webView.EvaluateJavascript(@$"
                document.documentElement.style.setProperty('--safe-area-inset-top','{Utilities.PxToDip(safeAreaInsetTop)}px');
                document.documentElement.style.setProperty('--safe-area-inset-bottom','{Utilities.PxToDip(safeAreaInsetBottom)}px');                
                document.documentElement.style.setProperty('--safe-area-inset-left','{Utilities.PxToDip(safeAreaInsetLeft)}px');
                document.documentElement.style.setProperty('--safe-area-inset-right','{Utilities.PxToDip(safeAreaInsetRight)}px');
            ", null);
        }
    }
}
