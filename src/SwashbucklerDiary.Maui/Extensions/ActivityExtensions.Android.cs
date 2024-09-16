using Android.App;
using AndroidX.Core.View;
using static Android.Resource;

#nullable disable
namespace SwashbucklerDiary.Maui.Extensions
{
    public static class ActivityExtensions
    {
        private static readonly AndroidX.Core.Graphics.Insets emptyInsets = AndroidX.Core.Graphics.Insets.Of(0, 0, 0, 0);

        public static AndroidX.Core.Graphics.Insets GetStatusBarInsets(this Activity activity)
        {
            Android.Views.View decorView = activity.FindViewById(Id.Content);
            WindowInsetsCompat windowInsets = ViewCompat.GetRootWindowInsets(decorView);
            if (windowInsets is null)
            {
                return emptyInsets;
            }

            return windowInsets.GetInsets(WindowInsetsCompat.Type.StatusBars());
        }

        public static AndroidX.Core.Graphics.Insets GetNavigationBarInsets(this Activity activity)
        {
            Android.Views.View decorView = activity.FindViewById(Id.Content);
            WindowInsetsCompat windowInsets = ViewCompat.GetRootWindowInsets(decorView);
            if (windowInsets is null)
            {
                return emptyInsets;
            }

            return windowInsets.GetInsets(WindowInsetsCompat.Type.NavigationBars());
        }

        public static int PxToDip(this Activity activity, float value)
        {
            float scale = activity.Resources.DisplayMetrics.Density;
            return (int)(value / scale + 0.5f);
        }
    }
}
