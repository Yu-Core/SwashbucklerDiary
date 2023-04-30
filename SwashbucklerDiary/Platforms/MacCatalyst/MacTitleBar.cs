namespace SwashbucklerDiary
{
#nullable disable
#pragma warning disable CA1416
    public static class MacTitleBar
    {
        public static void SetTitleBarColorForMac(Color backgroundColor, Color foregroundColor)
        {
            var res = Application.Current.Resources;
            res["PageBackgroundColor"] = backgroundColor;
            res["PrimaryTextColor"] = foregroundColor;
        }

        public static void InitTitleBarForMac(object sender, EventArgs e)
        {
#if MACCATALYST14_2_OR_GREATER
            var window = sender as Window;
            var uiWindow = window.Handler.PlatformView as UIKit.UIWindow;

            if (uiWindow != null)
            {
                uiWindow.WindowScene.Titlebar.TitleVisibility = UIKit.UITitlebarTitleVisibility.Hidden;
            }
#endif
        }
    }
}
