using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Maui.Platform;

#if MACCATALYST
using Foundation;
using WebKit;
using ObjCRuntime;
#endif

namespace SwashbucklerDiary.Maui
{
    public partial class MainPage
    {
        private partial void BlazorWebViewInitializing(object? sender, BlazorWebViewInitializingEventArgs e)
        {
            e.Configuration.AllowsInlineMediaPlayback = true;
            e.Configuration.MediaTypesRequiringUserActionForPlayback = WebKit.WKAudiovisualMediaTypes.None;
#if MACCATALYST
            e.Configuration.UserContentController.AddScriptMessageHandler(new FullscreenHandler(), "fullscreenHandler");
            e.Configuration.UserContentController.AddUserScript(new WKUserScript(new NSString(@"
				const notifyFullscreenChange = (isEnterFullscreen) => {
					window.webkit.messageHandlers.fullscreenHandler.postMessage(isEnterFullscreen);
				};
				document.addEventListener('webkitbeginfullscreen', () => notifyFullscreenChange(true), true);
				document.addEventListener('webkitendfullscreen', () => notifyFullscreenChange(false), true);
			"), WKUserScriptInjectionTime.AtDocumentEnd, true));
#endif
        }

        private partial void BlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e)
        {
            e.WebView.ScrollView.ShowsVerticalScrollIndicator = false; // 关闭滚动条
            e.WebView.BackgroundColor = _backgroundColor.ToPlatform();
        }
#if MACCATALYST


        private sealed class FullscreenHandler : WKScriptMessageHandler
        {
            public override void DidReceiveScriptMessage(WKUserContentController userContentController, WKScriptMessage message)
            {
                if (message.Body is NSNumber number)
                {
                    bool isEnterFullscreen = number.BoolValue;
                    MainThread.BeginInvokeOnMainThread(() => ToggleFullscreen(isEnterFullscreen));
                }
            }

            private static void ToggleFullscreen(bool isEnterFullscreen)
            {
                var nsAppClass = new Class("NSApplication");
                var sharedAppSel = new Selector("sharedApplication");
                var nsApp = Messaging.IntPtr_objc_msgSend(nsAppClass.Handle, sharedAppSel.Handle);

                if (nsApp == IntPtr.Zero) return;

                var windowsSel = new Selector("windows");
                var windowsPtr = Messaging.IntPtr_objc_msgSend(nsApp, windowsSel.Handle);
                var windows = Runtime.GetNSObject<NSArray>(windowsPtr);

                if (windows == null || windows.Count == 0) return;

                var nsWindow = windows.GetItem<NSObject>(0);
                if (nsWindow == null) return;

                var handle = nsWindow.Handle;
                var styleMaskSel = new Selector("styleMask");
                var styleMask = Messaging.nuint_objc_msgSend(handle, styleMaskSel.Handle);
                bool isFullscreen = (styleMask & 16384) == 16384; // NSWindowStyleMaskFullScreen = 16384
                if (isEnterFullscreen != isFullscreen)
                {
                    var toggleFullScreenSel = new Selector("toggleFullScreen:");
                    Messaging.void_objc_msgSend_IntPtr(handle, toggleFullScreenSel.Handle, IntPtr.Zero);
                }
            }
        }

        static partial class Messaging
        {
            private const string LIBOBJC = "/usr/lib/libobjc.dylib";

            [System.Runtime.InteropServices.LibraryImport(LIBOBJC, EntryPoint = "objc_msgSend")]
            public static partial IntPtr IntPtr_objc_msgSend(IntPtr receiver, IntPtr selector);

            [System.Runtime.InteropServices.LibraryImport(LIBOBJC, EntryPoint = "objc_msgSend")]
            public static partial nuint nuint_objc_msgSend(IntPtr receiver, IntPtr selector);

            [System.Runtime.InteropServices.LibraryImport(LIBOBJC, EntryPoint = "objc_msgSend")]
            public static partial void void_objc_msgSend_IntPtr(IntPtr receiver, IntPtr selector, IntPtr arg1);
        }
#endif
    }
}
