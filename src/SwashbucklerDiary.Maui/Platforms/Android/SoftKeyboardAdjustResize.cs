using Android.Widget;
using SwashbucklerDiary.Maui.Extensions;
using static Android.Resource;
using Activity = Android.App.Activity;
using Rect = Android.Graphics.Rect;
using View = Android.Views.View;

namespace SwashbucklerDiary.Maui
{
#nullable disable
    public class SoftKeyboardAdjustResize : IDisposable
    {
        private readonly WeakReference<Activity> _activityRef;
        private readonly bool _edgeToEdge;
        private readonly FrameLayout.LayoutParams _frameLayoutParams;
        private readonly View _childOfContent;
        private readonly Rect _rect = new();
        private int _usableHeightPrevious;
        private bool _disposed;

        public SoftKeyboardAdjustResize(Activity activity, bool edgeToEdge = true)
        {
            ArgumentNullException.ThrowIfNull(activity);

            _activityRef = new WeakReference<Activity>(activity);
            _edgeToEdge = edgeToEdge;

            var contentFrameLayout = activity.FindViewById<FrameLayout>(Id.Content)
                ?? throw new InvalidOperationException("Content FrameLayout not found.");

            _childOfContent = contentFrameLayout.GetChildAt(0)
                ?? throw new InvalidOperationException("Content FrameLayout has no child.");

            _frameLayoutParams = (FrameLayout.LayoutParams)_childOfContent.LayoutParameters
                ?? throw new InvalidOperationException("Child view does not have LayoutParameters.");

            _childOfContent.ViewTreeObserver.GlobalLayout += HandleGlobalLayout;
            SetBackgroundColor(Android.Graphics.Color.White);
        }

        private void HandleGlobalLayout(object sender, EventArgs e)
        {
            PossiblyResizeChildOfContent();
        }

        public void OnStop()
        {
            _childOfContent.RequestLayout();
        }

        private void PossiblyResizeChildOfContent()
        {
            if (!_activityRef.TryGetTarget(out Activity activity))
                return;

            activity.FindViewById<FrameLayout>(Id.Content)?.GetWindowVisibleDisplayFrame(_rect);
            var usableHeightNow = _rect.Height();

            if (usableHeightNow == _usableHeightPrevious)
                return;

            var rootView = _childOfContent.RootView;
            if (rootView == null)
                return;

            int usableHeightSansKeyboard = rootView.Height;
            int heightDifference = usableHeightSansKeyboard - usableHeightNow;

            if (_edgeToEdge)
            {
                var statusBarInsets = activity.GetStatusBarInsets();
                var navigationBarInsets = activity.GetNavigationBarInsets();
                _frameLayoutParams.Height = usableHeightNow + statusBarInsets.Top + navigationBarInsets.Bottom;

                if (heightDifference > usableHeightSansKeyboard / 4)
                {
                    rootView.Top = -statusBarInsets.Top;
                    _childOfContent.Layout(_rect.Left, _rect.Top, _rect.Right, _rect.Bottom);
                }
            }
            else
            {
                _frameLayoutParams.Height = usableHeightNow;
            }

            _childOfContent.RequestLayout();
            _usableHeightPrevious = usableHeightNow;
        }

        private void SetBackgroundColor(Android.Graphics.Color color)
        {
            _childOfContent.RootView?.SetBackgroundColor(color);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    if (_childOfContent != null)
                    {
                        _childOfContent.ViewTreeObserver.GlobalLayout -= HandleGlobalLayout;
                    }
                }
                _disposed = true;
            }
        }
    }
}