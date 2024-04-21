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
            // Trigger when returning to the app
            Application.Current.Windows[0].Resumed += (s, args) => SetSafeAreaCss(webView);
            // Trigger when screen rotates
            DeviceDisplay.Current.MainDisplayInfoChanged += async (sender, args) =>
            {
                await Task.Delay(200);
                SetSafeAreaCss(webView);
            };
        }

        private static void SetSafeAreaCss(Android.Webkit.WebView webView)
        {
            int statusBarHeight = Utilities.GetStatusBarInsets().Top;
            AndroidX.Core.Graphics.Insets navigationBarInsets = Utilities.GetNavigationBarInsets();
            int navigationBarHeight;
            // DeviceDisplay.Current.MainDisplayInfo.Orientation is inaccurate first entry, It must be Portrait
            if (DeviceDisplay.Current.MainDisplayInfo.Width > DeviceDisplay.Current.MainDisplayInfo.Height)
            {
                navigationBarHeight = 0;
            }
            else
            {
                navigationBarHeight = navigationBarInsets.Bottom;
            }
            int leftSafeAreaWidth = navigationBarInsets.Left;
            int rightSafeAreaWidth = navigationBarInsets.Right;
            SetSafeAreaCss(webView, statusBarHeight, navigationBarHeight, leftSafeAreaWidth, rightSafeAreaWidth);
        }

        private static void SetSafeAreaCss(Android.Webkit.WebView webView, int statusBarHeight, int navigationBarHeight, int leftSafeAreaWidth, int rightSafeAreaWidth)
        {
            webView.EvaluateJavascript(@$"
                document.documentElement.style.setProperty('--android-status-bar-height','{Utilities.PxToDip(statusBarHeight)}px');
                document.documentElement.style.setProperty('--android-navigation-bar-height','{Utilities.PxToDip(navigationBarHeight)}px');                
                document.documentElement.style.setProperty('--android-safe-area-left-width','{Utilities.PxToDip(leftSafeAreaWidth)}px');
                document.documentElement.style.setProperty('--android-safe-area-right-width','{Utilities.PxToDip(rightSafeAreaWidth)}px');
            ", null);
        }
    }
}
