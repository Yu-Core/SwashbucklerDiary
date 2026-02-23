using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Server.Middleware
{
    public class StartPathMiddleware
    {
        private readonly RequestDelegate _next;
        public StartPathMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, RouteMatcher routeMatcher)
        {
            var route = context.Request.Path.ToString();
            if (route == "/not-found" || !routeMatcher.IsMatch(route))
            {
                await _next(context);
                return;
            }

            //Welcome Page
            bool firstSetLanguage = Microsoft.Maui.Storage.Preferences.Default.Get<bool>(nameof(Setting.FirstSetLanguage), false);
            bool firstAgree = Microsoft.Maui.Storage.Preferences.Default.Get<bool>(nameof(Setting.FirstAgree), false);
            if (!firstSetLanguage || !firstAgree)
            {
                if (route != "/welcome")
                {
                    context.Response.Redirect("/welcome");
                    return;
                }

                await _next(context);
                return;
            }

            if (route == "/")
            {
                var quickRecord = Microsoft.Maui.Storage.Preferences.Default.Get<bool>(nameof(Setting.QuickRecord), false);
                if (quickRecord)
                {
                    context.Response.Redirect("/write");
                    return;
                }
            }

            if (route != "/appLock")
            {
                string appLockNumberPassword = Microsoft.Maui.Storage.Preferences.Default.Get<string>(nameof(Setting.AppLockNumberPassword), string.Empty);
                string appLockPatternPassword = Microsoft.Maui.Storage.Preferences.Default.Get<string>(nameof(Setting.AppLockPatternPassword), string.Empty);
                bool appLockBiometric = Microsoft.Maui.Storage.Preferences.Default.Get<bool>(nameof(Setting.AppLockBiometric), false);
                bool useAppLock = !string.IsNullOrEmpty(appLockNumberPassword)
                    || !string.IsNullOrEmpty(appLockPatternPassword)
                    || appLockBiometric;
                if (useAppLock)
                {
                    var returnUrl = $"{context.Request.Path}{context.Request.QueryString}".TrimStart('/');

                    context.Response.Redirect($"/appLock?returnUrl={Uri.EscapeDataString(returnUrl)}");
                    return;
                }
            }

            await _next(context);
        }
    }
}
