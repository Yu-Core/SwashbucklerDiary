using CoreGraphics;
using Foundation;
using Microsoft.Maui.Platform;
using UIKit;

namespace SwashbucklerDiary.Maui
{
#nullable disable
    public partial class MainPage
    {
        double paddingBottom = 0;

        bool showSoftKeyboard;

        NSObject _keyboardShowObserver;

        NSObject _keyboardHideObserver;

        ~MainPage()
        {
            UnregisterForKeyboardNotifications();
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
            if (showSoftKeyboard)
            {
                return;
            }

            showSoftKeyboard = true;
            NSValue result = (NSValue)args.Notification.UserInfo.ObjectForKey(new NSString(UIKeyboard.FrameEndUserInfoKey));
            CGSize keyboardSize = result.RectangleFValue.Size;

            paddingBottom = this.Padding.Bottom;
            this.Padding = new Thickness(Padding.Left, Padding.Top, Padding.Right, keyboardSize.Height);
        }

        void OnKeyboardHide(object sender, UIKeyboardEventArgs args)
        {
            if (!showSoftKeyboard)
            {
                return;
            }

            showSoftKeyboard = false;

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
