using Microsoft.Maui.Platform;
using PInvoke;
using WinRT.Interop;
using static PInvoke.User32;

namespace SwashbucklerDiary
{
# nullable disable
    public static class WindowsTitleBar
    {
        private static Microsoft.UI.Xaml.Window GetActiveNativeWindow() =>
        (Microsoft.UI.Xaml.Window)Application.Current.Windows.FirstOrDefault()?.Handler?.PlatformView;

        public static void SetColorForWindows(Color backgroundColor,Color foregroundColor)
        {
            var res = Microsoft.UI.Xaml.Application.Current.Resources;
            res["WindowCaptionBackground"] = backgroundColor.ToWindowsColor();
            res["WindowCaptionBackgroundDisabled"] = backgroundColor.ToWindowsColor();
            res["WindowCaptionForeground"] = foregroundColor.ToWindowsColor();
            res["WindowCaptionForegroundDisabled"] = foregroundColor.ToWindowsColor();
            TriggertTitleBarRepaint();
        }

        private static bool TriggertTitleBarRepaint()
        {
#if WINDOWS
            var nativeWindow = GetActiveNativeWindow();
            if (nativeWindow is null) 
            { 
                return default; 
            }

            var hWnd = WindowNative.GetWindowHandle(nativeWindow);
            var activeWindow = User32.GetActiveWindow();
            if (hWnd == activeWindow)
            {
                User32.PostMessage(hWnd, WindowMessage.WM_ACTIVATE, new IntPtr((int)0x00), IntPtr.Zero);
                User32.PostMessage(hWnd, WindowMessage.WM_ACTIVATE, new IntPtr((int)0x01), IntPtr.Zero);
            }
            else
            {
                User32.PostMessage(hWnd, WindowMessage.WM_ACTIVATE, new IntPtr((int)0x01), IntPtr.Zero);
                User32.PostMessage(hWnd, WindowMessage.WM_ACTIVATE, new IntPtr((int)0x00), IntPtr.Zero);
            }

#endif
            return true;
        }
    }
}
