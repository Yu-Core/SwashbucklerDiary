using Android.Widget;
using SwashbucklerDiary.Rcl;
using SwashbucklerDiary.Rcl.Essentials;
using static Android.Resource;
using Activity = Android.App.Activity;
using Rect = Android.Graphics.Rect;
using View = Android.Views.View;

namespace SwashbucklerDiary.Maui
{
#nullable disable
    public static class SoftKeyboardAdjustResize
    {
        static Activity Activity => Platform.CurrentActivity ?? throw new InvalidOperationException("Android Activity can't be null.");
        static View mChildOfContent;
        static FrameLayout.LayoutParams frameLayoutParams;
        static int usableHeightPrevious = 0;
        static readonly Rect rect = new();

        public static void Initialize()
        {
            FrameLayout content = (FrameLayout)Activity.FindViewById(Id.Content);
            mChildOfContent = content.GetChildAt(0);
            mChildOfContent.ViewTreeObserver.GlobalLayout += (s, o) => PossiblyResizeChildOfContent();
            frameLayoutParams = (FrameLayout.LayoutParams)mChildOfContent?.LayoutParameters;
            SetBackgroundColor();
        }

        static void PossiblyResizeChildOfContent()
        {
            ((FrameLayout)Activity.FindViewById(Id.Content)).GetWindowVisibleDisplayFrame(rect);
            var usableHeightNow = rect.Height();
            if (usableHeightNow != usableHeightPrevious)
            {
                frameLayoutParams.Height = usableHeightNow + Utilities.GetStatusBarInsets().Top + Utilities.GetNavigationBarInsets().Bottom;
                mChildOfContent.RootView.Top = -Utilities.GetStatusBarInsets().Top;

                mChildOfContent.Layout(rect.Left, rect.Top, rect.Right, rect.Bottom);
                mChildOfContent.RequestLayout();
                usableHeightPrevious = usableHeightNow;
            }
        }

        static readonly Android.Graphics.Color lightColor = Android.Graphics.Color.ParseColor(ThemeColor.LightSurface);

        static readonly Android.Graphics.Color darkColor = Android.Graphics.Color.ParseColor(ThemeColor.DarkSurface);

        static void SetBackgroundColor()
        {
            var themeService = IPlatformApplication.Current!.Services.GetRequiredService<IThemeService>();
            themeService.OnChanged += (theme) =>
            {
                var color = theme == Shared.Theme.Dark ? darkColor : lightColor;
                mChildOfContent.RootView.SetBackgroundColor(color);
            };
        }
    }
}
