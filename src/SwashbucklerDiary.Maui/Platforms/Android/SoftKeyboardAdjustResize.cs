using Android.Widget;
using static Android.Resource;
using Activity = Android.App.Activity;
using Rect = Android.Graphics.Rect;
using View = Android.Views.View;

namespace SwashbucklerDiary.Maui
{
#nullable disable
    public static class SoftKeyboardAdjustResize
    {
        static Activity Activity => Microsoft.Maui.ApplicationModel.Platform.CurrentActivity ?? throw new InvalidOperationException("Android Activity can't be null.");
        static View mChildOfContent;
        static FrameLayout.LayoutParams frameLayoutParams;
        static int usableHeightPrevious = 0;

        public static void Initialize()
        {
            FrameLayout content = (FrameLayout)Activity.FindViewById(Id.Content);
            mChildOfContent = content.GetChildAt(0);
            mChildOfContent.ViewTreeObserver.GlobalLayout += (s, o) => PossiblyResizeChildOfContent();
            frameLayoutParams = (FrameLayout.LayoutParams)mChildOfContent?.LayoutParameters;
        }

        static void PossiblyResizeChildOfContent()
        {
            int usableHeightNow = ComputeUsableHeight();
            if (usableHeightNow != usableHeightPrevious)
            {
                int usableHeightSansKeyboard = mChildOfContent.RootView.Height;
                int heightDifference = usableHeightSansKeyboard - usableHeightNow;
                if (heightDifference < 0)
                {
                    usableHeightSansKeyboard = mChildOfContent.RootView.Width;
                    heightDifference = usableHeightSansKeyboard - usableHeightNow;
                }

                if (heightDifference > usableHeightSansKeyboard / 4)
                {
                    frameLayoutParams.Height = usableHeightSansKeyboard - heightDifference + Utilities.GetNavigationBarInsets().Bottom;
                }
                else if (heightDifference >= Utilities.GetNavigationBarInsets().Bottom)
                {
                    frameLayoutParams.Height = usableHeightSansKeyboard;
                }
                else
                {
                    frameLayoutParams.Height = usableHeightNow;
                }
            }

            mChildOfContent.RequestLayout();
            usableHeightPrevious = usableHeightNow;
        }

        static int ComputeUsableHeight()
        {
            Rect rect = new Rect();
            mChildOfContent.GetWindowVisibleDisplayFrame(rect);
            return rect.Bottom;
        }
    }
}
