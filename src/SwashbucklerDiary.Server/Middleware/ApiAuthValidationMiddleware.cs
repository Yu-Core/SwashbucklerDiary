using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Server.Attributes;
using SwashbucklerDiary.Server.Services;

namespace SwashbucklerDiary.Server.Middleware
{
    public class ApiAuthValidationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IApiAuthService _apiAuthService;
        private readonly RouteMatcher _routeMatcher;

        public ApiAuthValidationMiddleware(RequestDelegate next,
            IApiAuthService apiAuthService,
            RouteMatcher routeMatcher)
        {
            _next = next;
            _apiAuthService = apiAuthService;
            _routeMatcher = routeMatcher;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var route = context.Request.Path.ToString();
            if (route != "/not-found" &&
                route != "/appLock" &&
                _routeMatcher.IsMatch(route))
            {
                var cookie = _apiAuthService.GenerateApiKey();
                context.Response.Cookies.Append("ApiAuth", cookie, new CookieOptions
                {
                    HttpOnly = true,
                    //Secure = true,
                    SameSite = SameSiteMode.Strict,
                    //Expires = DateTimeOffset.UtcNow.AddDays(365)
                });

                await _next(context);
                return;
            }

            var endpoint = context.GetEndpoint();

            if (endpoint != null)
            {
                var hasAttr = endpoint.Metadata.GetMetadata<ApiAuthAttribute>() != null;

                if (hasAttr)
                {
                    if (!context.Request.Cookies.TryGetValue("ApiAuth", out var cookieValue))
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Missing Cookie");
                        return;
                    }

                    if (!_apiAuthService.ValidateApiKey(cookieValue))
                    {
                        context.Response.StatusCode = 401;
                        await context.Response.WriteAsync("Invalid Cookie");
                        return;
                    }
                }
            }

            await _next(context);
        }
    }
}
