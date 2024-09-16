using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Extensions.Logging;

namespace SwashbucklerDiary.Maui.BlazorWebView
{
    public static class Log
    {
        private static readonly Type logType = typeof(BlazorWebViewHandler).Assembly.GetType("Microsoft.AspNetCore.Components.WebView.Log")
            ?? throw new Exception("Type Microsoft.AspNetCore.Components.WebView.Log does not exist");

        public static void CreatingWebKitWKWebView(this ILogger logger)
        {
            var method = logType.GetMethod("CreatingWebKitWKWebView") ?? throw new Exception("Method CreatingWebKitWKWebView does not exist");
            method.Invoke(null, [logger]);
        }

        public static void CreatedWebKitWKWebView(this ILogger logger)
        {
            var method = logType.GetMethod("CreatedWebKitWKWebView") ?? throw new Exception("Method CreatedWebKitWKWebView does not exist");
            method.Invoke(null, [logger]);
        }

        public static void HandlingWebRequest(this ILogger logger, string url)
        {
            var method = logType.GetMethod("HandlingWebRequest") ?? throw new Exception("Method HandlingWebRequest does not exist");
            method.Invoke(null, [logger, url]);
        }

        public static void ResponseContentBeingSent(this ILogger logger, string url, int statusCode)
        {
            var method = logType.GetMethod("ResponseContentBeingSent") ?? throw new Exception("Method ResponseContentBeingSent does not exist");
            method.Invoke(null, [logger, url, statusCode]);
        }

        public static void ReponseContentNotFound(this ILogger logger, string url)
        {
            var method = logType.GetMethod("ReponseContentNotFound") ?? throw new Exception("Method ReponseContentNotFound does not exist");
            method.Invoke(null, [logger, url]);
        }
    }
}
