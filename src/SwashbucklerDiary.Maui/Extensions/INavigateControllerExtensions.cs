using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Maui.Extensions
{
    public static class INavigateControllerExtensions
    {
        public static bool CheckUrlScheme(this INavigateController navigateController, string? uriString, out string path)
        {
            if (Uri.TryCreate(uriString, UriKind.Absolute, out var uri))
            {
                var route = $"/{uri.Host}{uri.AbsolutePath.TrimEnd('/')}";
                if (navigateController.RouteMatcher.IsMatch(route))
                {
                    path = $"{route}{uri.Query}{uri.Fragment}";
                    return true;
                }
            }

            path = string.Empty;
            return false;
        }
    }
}
