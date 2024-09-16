using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Maui.Extensions
{
    public static class INavigateControllerExtensions
    {
        public static bool CheckUrlScheme(this INavigateController navigateController, string? uriString, out string path)
        {
            if (Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
            {
                var resolvedPath = $"/{uri.Host}{uri.AbsolutePath.TrimEnd('/')}{uri.Query}{uri.Fragment}";
                if (navigateController.RouteMatcher.IsMatch(resolvedPath))
                {
                    path = resolvedPath;
                    return true;
                }
            }

            path = string.Empty;
            return false;
        }
    }
}
