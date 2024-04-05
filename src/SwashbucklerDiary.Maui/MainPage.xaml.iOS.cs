using CoreGraphics;
using Foundation;
using Microsoft.Maui.Platform;
using UIKit;

namespace SwashbucklerDiary.Maui
{
#nullable disable
    public partial class MainPage
    {
        double paddingBottom = 10;

        NSObject _keyboardShowObserver;

        NSObject _keyboardHideObserver;

        static UIView statusBar;

        ~MainPage()
        {
            UnregisterForKeyboardNotifications();
        }

        public static void SetIOSGapColor(Color color)
        {
            statusBar.BackgroundColor = color.ToPlatform();
        }

        // On the iOS platform, white gap/field below status bar
        // https://github.com/dotnet/maui/issues/19778
        protected override void OnHandlerChanged()
        {
            base.OnHandlerChanged();

            var window = this.GetParentWindow()?.Handler?.PlatformView as UIWindow;
            if (window is not null)
            {
                var topPadding = window?.SafeAreaInsets.Top ?? 0;

                statusBar = new UIView(new CGRect(0, 0, UIScreen.MainScreen.Bounds.Size.Width, topPadding));

                var view = this.Handler?.PlatformView as UIView;
                if (view is not null)
                {
                    view?.AddSubview(statusBar);
                }

                DeviceDisplay.Current.MainDisplayInfoChanged += (sender, args) =>
                {
                    var orientation = args.DisplayInfo.Orientation;
                    if (orientation == DisplayOrientation.Landscape)
                    {
                        statusBar.Frame = new(0, 0, UIScreen.MainScreen.Bounds.Size.Width, 0);
                    }
                    else if (orientation == DisplayOrientation.Portrait)
                    {
                        topPadding = window?.SafeAreaInsets.Top ?? 0;
                        statusBar.Frame = new(0, 0, UIScreen.MainScreen.Bounds.Size.Width, topPadding);
                    }
                };
            }
        }

        void Initialize()
        {
            this.Padding = new(Padding.Left, Padding.Top, Padding.Right, paddingBottom);
            RegisterForKeyboardNotifications();
        }

        // On the iOS platform, adjust the window size when the soft keyboard pops up
        // https://github.com/dotnet/maui/issues/10662
        void OnKeyboardShow(object sender, UIKeyboardEventArgs args)
        {
            NSValue result = (NSValue)args.Notification.UserInfo.ObjectForKey(new NSString(UIKeyboard.FrameEndUserInfoKey));
            CGSize keyboardSize = result.RectangleFValue.Size;

            paddingBottom = this.Padding.Bottom;
            this.Padding = new Thickness(Padding.Left, Padding.Top, Padding.Right, keyboardSize.Height);
        }

        void OnKeyboardHide(object sender, UIKeyboardEventArgs args)
        {
            this.Padding = new Thickness(Padding.Left, Padding.Top, Padding.Right, paddingBottom);
        }

        void RegisterForKeyboardNotifications()
        {
            _keyboardShowObserver ??= UIKeyboard.Notifications.ObserveWillShow(OnKeyboardShow);
            _keyboardHideObserver ??= UIKeyboard.Notifications.ObserveWillHide(OnKeyboardHide);
        }

        void UnregisterForKeyboardNotifications()
        {
            if (_keyboardShowObserver is not null)
            {
                _keyboardShowObserver.Dispose();
                _keyboardShowObserver = null;
            }

            if (_keyboardHideObserver is not null)
            {
                _keyboardHideObserver.Dispose();
                _keyboardHideObserver = null;
            }
        }
    }
}
