using Android.Widget;
using SwashbucklerDiary.Maui.Extensions;
using static Android.Resource;
using Activity = Android.App.Activity;
using Rect = Android.Graphics.Rect;
using View = Android.Views.View;

namespace SwashbucklerDiary.Maui
{
#nullable disable
    public class SoftKeyboardAdjustResize
    {
        Activity _activity;
        bool _edgeToEdge;
        FrameLayout.LayoutParams frameLayoutParams;
        int usableHeightPrevious = 0;
        Rect rect = new();
        View mChildOfContent;

        public SoftKeyboardAdjustResize(Activity activity, bool edgeToEdge)
        {
            _activity = activity;
            _edgeToEdge = edgeToEdge;
            mChildOfContent = _activity.FindViewById<FrameLayout>(Id.Content).GetChildAt(0);
            mChildOfContent.ViewTreeObserver.GlobalLayout += (s, o) => PossiblyResizeChildOfContent();
            frameLayoutParams = (FrameLayout.LayoutParams)mChildOfContent?.LayoutParameters;
            SetBackgroundColor(Android.Graphics.Color.White);
        }

        public static void AssistActivity(Activity activity, bool edgeToEdge = true)
        {
            _ = new SoftKeyboardAdjustResize(activity, edgeToEdge);
        }

        void PossiblyResizeChildOfContent()
        {
            _activity.FindViewById<FrameLayout>(Id.Content).GetWindowVisibleDisplayFrame(rect);
            var usableHeightNow = rect.Height();
            if (usableHeightNow != usableHeightPrevious)
            {
                if (_edgeToEdge)
                {
                    frameLayoutParams.Height = usableHeightNow + _activity.GetStatusBarInsets().Top + _activity.GetNavigationBarInsets().Bottom;
                }
                else
                {
                    frameLayoutParams.Height = usableHeightNow;
                }

                //Resolve anomalies during screen rotation
                mChildOfContent.RootView.Top = -_activity.GetStatusBarInsets().Top;
                mChildOfContent.Layout(rect.Left, rect.Top, rect.Right, rect.Bottom);

                mChildOfContent.RequestLayout();
                usableHeightPrevious = usableHeightNow;
            }
        }

        void SetBackgroundColor(Android.Graphics.Color color)
        {
            mChildOfContent.RootView.SetBackgroundColor(color);
        }
    }
}
