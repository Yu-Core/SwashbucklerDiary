using AndroidX.Core.View;
using static Android.Resource;

namespace SwashbucklerDiary.Maui
{
#nullable disable
    public static class Utilities
    {
        public static AndroidX.Core.Graphics.Insets GetStatusBarInsets()
        {
            Android.Views.View decorView = Platform.CurrentActivity.FindViewById(Id.Content);
            WindowInsetsCompat windowInsets = ViewCompat.GetRootWindowInsets(decorView);
            return windowInsets.GetInsets(WindowInsetsCompat.Type.StatusBars());
        }

        public static AndroidX.Core.Graphics.Insets GetNavigationBarInsets()
        {
            Android.Views.View decorView = Platform.CurrentActivity.FindViewById(Id.Content);
            WindowInsetsCompat windowInsets = ViewCompat.GetRootWindowInsets(decorView);
            return windowInsets.GetInsets(WindowInsetsCompat.Type.NavigationBars());
        }

        public static AndroidX.Core.Graphics.Insets GetSoftKeyboardInsets()
        {
            Android.Views.View decorView = Platform.CurrentActivity.FindViewById(Id.Content);
            WindowInsetsCompat windowInsets = ViewCompat.GetRootWindowInsets(decorView);
            return windowInsets.GetInsets(WindowInsetsCompat.Type.Ime());
        }

        public static int PxToDip(float value)
        {
            float scale = Platform.CurrentActivity.Resources.DisplayMetrics.Density;
            return (int)(value / scale + 0.5f);
        }
    }
}
