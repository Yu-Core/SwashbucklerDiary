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
            DeviceDisplay.Current.MainDisplayInfoChanged += (sender, args) => SetSafeAreaCss(webView);
            SoftKeyboardAdjustResize.SoftKeyboardLifting += () => SetSafeAreaCss(webView, Utilities.GetStatusBarHeight(), 0);
            SoftKeyboardAdjustResize.SoftKeyboardDroping += () => SetSafeAreaCss(webView);
        }

        private static void SetSafeAreaCss(Android.Webkit.WebView webView)
        {
            int statusBarHeight = Utilities.GetStatusBarHeight();
            int navigationBarHeight = Utilities.GetNavigationBarHeight();

            SetSafeAreaCss(webView, statusBarHeight, navigationBarHeight);
        }

        private static void SetSafeAreaCss(Android.Webkit.WebView webView, int statusBarHeight, int navigationBarHeight)
        {
            webView.EvaluateJavascript(@$"
                document.documentElement.style.setProperty('--android-status-bar-height','{Utilities.PxToDip(statusBarHeight)}px');
                document.documentElement.style.setProperty('--android-navigation-bar-height','{Utilities.PxToDip(navigationBarHeight)}px');
            ", null);
        }
    }
}
