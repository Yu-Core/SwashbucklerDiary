using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static class RouteMatcherExtensions
    {
        public static bool CheckRouter(this RouteMatcher routeMatcher, string route)
        {
            if (route == "/welcome" || route == "/appLock")
            {
                return false;
            }

            if (routeMatcher.IsMatch(route))
            {
                return true;
            }

            return false;
        }
    }
}
