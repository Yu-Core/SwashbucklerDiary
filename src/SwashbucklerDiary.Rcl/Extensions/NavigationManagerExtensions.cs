using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static class NavigationManagerExtensions
    {
        public static bool IsBaseOf(this NavigationManager navigationManager, string url)
        {
            return new Uri(navigationManager.BaseUri).IsBaseOf(new Uri(url, UriKind.RelativeOrAbsolute));
        }

        public static string GetBaseRelativePath(this NavigationManager navigationManager)
        {
            return navigationManager.ToBaseRelativePath(navigationManager.Uri);
        }
    }
}
