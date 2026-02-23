using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;

namespace SwashbucklerDiary.Rcl.Web.Extensions
{
    public static class RouteMatcherExtensions
    {
        public static bool CheckUrlScheme(this RouteMatcher routeMatcher, NavigationManager navigationManager, string? uriString)
        {
            if (Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
            {
                var relativePath = navigationManager.ToBaseRelativePath(uri.GetLeftPart(UriPartial.Path));
                var route = $"/{relativePath.TrimEnd('/')}";

                if (routeMatcher.CheckRouter(route))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
