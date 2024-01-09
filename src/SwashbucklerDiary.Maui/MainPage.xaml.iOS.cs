using CoreGraphics;
using Foundation;
using UIKit;

namespace SwashbucklerDiary.Maui
{
#nullable disable
    public partial class MainPage
    {
        NSObject _keyboardShowObserver;
        
        NSObject _keyboardHideObserver;

        ~MainPage()
        {
            UnregisterForKeyboardNotifications();
        }

        //On the iOS platform, adjust the window size when the soft keyboard pops up
        //https://github.com/dotnet/maui/issues/10662
        void OnKeyboardShow(object sender, UIKeyboardEventArgs args)
        {
            NSValue result = (NSValue)args.Notification.UserInfo.ObjectForKey(new NSString(UIKeyboard.FrameEndUserInfoKey));
            CGSize keyboardSize = result.RectangleFValue.Size;

            this.Padding = new Thickness(0, 0, 0, keyboardSize.Height);
        }

        void OnKeyboardHide(object sender, UIKeyboardEventArgs args)
        {
            this.Padding = new Thickness(0);
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
