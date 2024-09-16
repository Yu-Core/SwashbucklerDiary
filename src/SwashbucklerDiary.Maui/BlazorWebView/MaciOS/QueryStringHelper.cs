using Microsoft.AspNetCore.Components.WebView.Maui;
using System.Reflection;

namespace SwashbucklerDiary.Maui.BlazorWebView
{
#nullable disable
    public static class QueryStringHelper
    {
        private static readonly Type queryStringHelperType = typeof(BlazorWebViewHandler).Assembly.GetType("Microsoft.AspNetCore.Components.WebView.QueryStringHelper")
            ?? throw new Exception("Type Microsoft.AspNetCore.Components.WebView.QueryStringHelper does not exist");

        private static readonly MethodInfo removePossibleQueryStringMethod = queryStringHelperType.GetMethod("RemovePossibleQueryString", BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static)
                ?? throw new Exception("Method RemovePossibleQueryString does not exist");

        public static string RemovePossibleQueryString(string? url)
        {
            return (string)removePossibleQueryStringMethod.Invoke(null, [url]);
        }
    }
}
