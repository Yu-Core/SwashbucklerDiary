using Android.App;
using Android.Widget;
using SwashbucklerDiary.Maui.Extensions;
using static Android.Resource;

namespace SwashbucklerDiary.Maui
{
#nullable disable
    public static class AndroidSafeArea
    {
        public static void Initialize(Android.Webkit.WebView webView)
        {
            var activity = webView.Context as Activity;
            SetSafeAreaCss(webView);
            FrameLayout content = activity.FindViewById<FrameLayout>(Id.Content);
            content.GetChildAt(0).ViewTreeObserver.GlobalLayout += (s, o) => SetSafeAreaCss(webView);
        }

        private static void SetSafeAreaCss(Android.Webkit.WebView webView)
        {
            var activity = webView.Context as Activity;
            int safeAreaInsetTop = activity.GetStatusBarInsets().Top;
            AndroidX.Core.Graphics.Insets navigationBarInsets = activity.GetNavigationBarInsets();
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

        private static void SetSafeAreaCss(Android.Webkit.WebView webView,
            int safeAreaInsetTop,
            int safeAreaInsetBottom,
            int safeAreaInsetLeft,
            int safeAreaInsetRight)
        {
            var activity = webView.Context as Activity;
            webView.EvaluateJavascript(@$"
                document.documentElement.style.setProperty('--safe-area-inset-top','{activity.PxToDip(safeAreaInsetTop)}px');
                document.documentElement.style.setProperty('--safe-area-inset-bottom','{activity.PxToDip(safeAreaInsetBottom)}px');                
                document.documentElement.style.setProperty('--safe-area-inset-left','{activity.PxToDip(safeAreaInsetLeft)}px');
                document.documentElement.style.setProperty('--safe-area-inset-right','{activity.PxToDip(safeAreaInsetRight)}px');
            ", null);
        }
    }
}
