using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static class NavigationManagerExtensions
    {
        public static bool IsBaseOf(this NavigationManager navigationManager, string uriString)
        {
            return new Uri(navigationManager.BaseUri).IsBaseOf(new Uri(uriString, UriKind.RelativeOrAbsolute));
        }

        public static string GetBaseRelativePath(this NavigationManager navigationManager)
        {
            return navigationManager.ToBaseRelativePath(navigationManager.Uri);
        }

        public static string ToRoute(this NavigationManager navigationManager, string uriString)
        {
            var absolutePath = new Uri(uriString).AbsolutePath;
            var basePath = new Uri(navigationManager.BaseUri).AbsolutePath;
            var route = absolutePath.Substring(basePath.Length - 1);
            return route;
        }
    }
}
