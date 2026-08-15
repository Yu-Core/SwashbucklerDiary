using Microsoft.Maui.Platform;
using System.Runtime.InteropServices;

namespace SwashbucklerDiary.Maui
{
    public class TitleBarButtonDirectionHelper
    {
        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_LAYOUTRTL = 0x00400000;

        [DllImport("user32.dll", EntryPoint = "GetWindowLongPtr")]
        private static extern IntPtr GetWindowLongPtr(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", EntryPoint = "SetWindowLongPtr")]
        private static extern IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        public static void UpdateTitleBarButtonDirection(Window window, bool isRightToLeft)
        {
            var nativeWindow = window.Handler.PlatformView as Microsoft.UI.Xaml.Window;
            if (nativeWindow is null) return;

            var hwnd = nativeWindow.GetWindowHandle();
            if (hwnd == IntPtr.Zero) return;

            var exStyle = GetWindowLongPtr(hwnd, GWL_EXSTYLE);
            if (isRightToLeft)
            {
                exStyle |= WS_EX_LAYOUTRTL;
            }
            else
            {
                exStyle &= ~WS_EX_LAYOUTRTL;
            }
            SetWindowLongPtr(hwnd, GWL_EXSTYLE, exStyle);
        }


    }
}
