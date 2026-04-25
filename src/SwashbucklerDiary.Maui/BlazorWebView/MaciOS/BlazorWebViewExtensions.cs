using Microsoft.AspNetCore.Components.WebView.Maui;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace SwashbucklerDiary.Maui
{
    public static class BlazorWebViewHelper
    {
        [DynamicDependency("AppHostAddress", typeof(BlazorWebView))]
        private static readonly PropertyInfo _appHostAddressProperty = typeof(BlazorWebView).GetProperty("AppHostAddress", BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException("Property 'AppHostAddress' not found on BlazorWebView");

        [DynamicDependency("AppOrigin", typeof(BlazorWebViewHandler))]
        private static readonly PropertyInfo _appOriginProperty = typeof(BlazorWebViewHandler).GetProperty("AppOrigin", BindingFlags.NonPublic | BindingFlags.Static)
            ?? throw new InvalidOperationException("Property 'AppOrigin' not found on BlazorWebViewHandler");

        public static string AppHostAddress => _appHostAddressProperty.GetValue(null) as string
            ?? throw new InvalidOperationException("Value of 'AppHostAddress' property is null");

        public static string AppOrigin => _appOriginProperty.GetValue(null) as string
            ?? throw new InvalidOperationException("Value of 'AppOrigin' property is null");
    }
}
