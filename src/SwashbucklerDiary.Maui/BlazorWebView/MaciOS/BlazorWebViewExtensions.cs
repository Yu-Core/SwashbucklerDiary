using Microsoft.AspNetCore.Components.WebView.Maui;
using System.Reflection;

namespace SwashbucklerDiary.Maui
{
    public static class BlazorWebViewHelper
    {
        private static readonly Lazy<PropertyInfo> _appHostAddressProperty = new Lazy<PropertyInfo>(() =>
        {
            var property = typeof(BlazorWebView).GetProperty("AppHostAddress",
                BindingFlags.NonPublic | BindingFlags.Static);
            return property ?? throw new InvalidOperationException(
                "Property 'AppHostAddress' not found on BlazorWebView");
        });

        private static readonly Lazy<PropertyInfo> _appOriginProperty = new Lazy<PropertyInfo>(() =>
        {
            var property = typeof(BlazorWebViewHandler).GetProperty("AppOrigin",
                BindingFlags.NonPublic | BindingFlags.Static);
            return property ?? throw new InvalidOperationException(
                "Property 'AppOrigin' not found on BlazorWebViewHandler");
        });

        public static string AppHostAddress
        {
            get
            {
                var value = _appHostAddressProperty.Value.GetValue(null) as string;
                return value ?? throw new InvalidOperationException(
                    "Value of 'AppHostAddress' property is null");
            }
        }

        public static string AppOrigin
        {
            get
            {
                var value = _appOriginProperty.Value.GetValue(null) as string;
                return value ?? throw new InvalidOperationException(
                    "Value of 'AppOrigin' property is null");
            }
        }
    }
}
