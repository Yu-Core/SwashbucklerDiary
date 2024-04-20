using AndroidX.Core.View;
using static Android.Resource;

namespace SwashbucklerDiary.Maui
{
#nullable disable
    public static class Utilities
    {
        public static int GetStatusBarHeight()
        {
            var resources = Platform.CurrentActivity.Resources;
            var id = resources.GetIdentifier("status_bar_height", "dimen", "android");
            if (id > 0)
            {
                return resources.GetDimensionPixelSize(id);
            }

            return 0;
        }

        public static int GetNavigationBarHeight()
        {
            if (DeviceDisplay.Current.MainDisplayInfo.Orientation == DisplayOrientation.Landscape)
            {
                return 0;
            }

            bool isNavigationBarExist = IsNavigationBarExist();
            if (isNavigationBarExist)
            {
                var resources = Platform.CurrentActivity.Resources;
                var id = resources.GetIdentifier("navigation_bar_height", "dimen", "android");
                if (id > 0)
                {
                    return resources.GetDimensionPixelSize(id);
                }
            }

            return 0;
        }

        private static bool IsNavigationBarExist()
        {
            Android.Views.View decorView = Platform.CurrentActivity.FindViewById(Id.Content);
            WindowInsetsCompat windowInsets = ViewCompat.GetRootWindowInsets(decorView);

            if (windowInsets != null)
            {

                bool hasNavigationBar = windowInsets.IsVisible(WindowInsetsCompat.Type.NavigationBars()) &&
                        windowInsets.GetInsets(WindowInsetsCompat.Type.NavigationBars()).Bottom > 0;

                return hasNavigationBar;
            }

            return false;
        }

        public static int PxToDip(float value)
        {
            float scale = Platform.CurrentActivity.Resources.DisplayMetrics.Density;
            return (int)(value / scale + 0.5f);
        }
    }
}
