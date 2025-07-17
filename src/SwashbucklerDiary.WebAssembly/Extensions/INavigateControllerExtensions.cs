using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.WebAssembly.Extensions
{
    public static class INavigateControllerExtensions
    {
        public static bool CheckUrlScheme(this INavigateController navigateController, NavigationManager navigationManager, string? uriString)
        {
            if (Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
            {
                var relativePath = navigationManager.ToBaseRelativePath(uri.ToString());
                var route = $"/{relativePath.TrimEnd('/')}";
                if (navigateController.RouteMatcher.IsMatch(route))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
