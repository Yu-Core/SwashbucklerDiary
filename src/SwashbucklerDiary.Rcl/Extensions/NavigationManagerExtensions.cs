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
            var abs = navigationManager.ToAbsoluteUri(uriString).ToString();
            return navigationManager.ToRouteCore(abs);
        }

        public static string GetRoute(this NavigationManager navigationManager)
        {
            return navigationManager.ToRouteCore(navigationManager.Uri);
        }

        private static string ToRouteCore(this NavigationManager navigationManager, string absoluteUri)
        {
            var relative = navigationManager.ToBaseRelativePath(absoluteUri);
            return '/' + relative.Split('?', '#')[0].TrimEnd('/');
        }
    }
}
