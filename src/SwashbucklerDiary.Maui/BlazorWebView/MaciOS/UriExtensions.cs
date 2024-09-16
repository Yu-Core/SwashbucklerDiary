using Microsoft.AspNetCore.Components.WebView.Maui;
using System.Reflection;

namespace SwashbucklerDiary.Maui.BlazorWebView
{
#nullable disable
    public static class UriExtensions
    {
        private static readonly Type uriExtensionsType = typeof(BlazorWebViewHandler).Assembly.GetType("Microsoft.AspNetCore.Components.WebView.Maui.UriExtensions")
            ?? throw new Exception("Type Microsoft.AspNetCore.Components.WebView.Maui.UriExtensions does not exist");

        private static readonly MethodInfo isBaseOfPageMethod = uriExtensionsType.GetMethod("IsBaseOfPage", BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
                ?? throw new Exception("Method IsBaseOfPage does not exist");

        public static bool IsBaseOfPage(this Uri uri, string? uriString)
        {
            return (bool)isBaseOfPageMethod.Invoke(null, [uri, uriString]);
        }
    }
}
