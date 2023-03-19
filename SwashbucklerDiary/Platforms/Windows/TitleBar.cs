using Microsoft.Maui.Platform;
using Microsoft.UI;           // Needed for WindowId.
using WinRT.Interop;
using AppWindow = Microsoft.UI.Windowing.AppWindow;
using AppWindowTitleBar = Microsoft.UI.Windowing.AppWindowTitleBar;

namespace SwashbucklerDiary
{
# nullable disable
    public static class TitleBar
    {
        public static Microsoft.UI.Xaml.Window GetActiveNativeWindow() =>
        (Microsoft.UI.Xaml.Window)Application.Current.Windows.FirstOrDefault()?.Handler?.PlatformView;

        public static AppWindow GetActiveAppWindow()
        {
            var nativeWindow = GetActiveNativeWindow();
            if (nativeWindow is null) { return default; }
            var hWnd = WindowNative.GetWindowHandle(nativeWindow);
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            return AppWindow.GetFromWindowId(windowId);
        }
        public static void SetColorForWindows(Color color)
        {
            // Check to see if customization is supported.
            // Currently only supported on Windows 11.
            if (AppWindowTitleBar.IsCustomizationSupported())
            {
                AppWindow appWindow = GetActiveAppWindow();
                var titleBar = appWindow.TitleBar;
                titleBar.BackgroundColor = color.ToWindowsColor();
            }
        }
    }
}
