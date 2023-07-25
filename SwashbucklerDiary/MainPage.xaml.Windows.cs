using Microsoft.AspNetCore.Components.WebView;
using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.Web.WebView2.Core;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;

namespace SwashbucklerDiary
{
    public partial class MainPage
    {
        private partial void BlazorWebViewInitializing(object? sender, BlazorWebViewInitializingEventArgs e)
        {
        }

        private partial void BlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e)
        {
            var webview2 = e.WebView.CoreWebView2;
            webview2.WebResourceRequested += WebView2WebResourceRequested;
        }

        async void WebView2WebResourceRequested(CoreWebView2 webview2, CoreWebView2WebResourceRequestedEventArgs args)
        {
            await HandleAppDataRequest(webview2,args);
        }

        async Task HandleAppDataRequest(CoreWebView2 webview2, CoreWebView2WebResourceRequestedEventArgs args)
        {
            string path = new Uri(args.Request.Uri).AbsolutePath.TrimStart('/');
            if(!path.StartsWith("appdata/"))
            {
                return;
            }
            else
            {
                path = path.TrimStart("appdata/".ToCharArray());
            }

            //path = path.Replace("/", "\\");
            path = Path.Combine(FileSystem.AppDataDirectory, path);
            if (File.Exists(path))
            {
                using var contentStream = File.OpenRead(path);
                IRandomAccessStream stream = await CopyContentToRandomAccessStreamAsync(contentStream);
                var contentType = StaticContentProvider.GetResponseContentTypeOrDefault(path);
                var headers = StaticContentProvider.GetResponseHeaders(contentType);
                var headerString = GetHeaderString(headers);
                var response = webview2.Environment.CreateWebResourceResponse(stream, 200, "OK", headerString);
                args.Response = response;
            }
            else
            {
                var response = webview2.Environment.CreateWebResourceResponse(null, 404, "Not Found", string.Empty);
                args.Response = response;
            }

            static string GetHeaderString(IDictionary<string, string> headers) =>
            string.Join(Environment.NewLine, headers.Select(kvp => $"{kvp.Key}: {kvp.Value}"));

            static async Task<IRandomAccessStream> CopyContentToRandomAccessStreamAsync(Stream content)
            {
                using var memStream = new MemoryStream();
                await content.CopyToAsync(memStream);
                var randomAccessStream = new InMemoryRandomAccessStream();
                await randomAccessStream.WriteAsync(memStream.GetWindowsRuntimeBuffer());
                return randomAccessStream;
            }
        }
    }
}
