using Microsoft.AspNetCore.Components.WebView;
using Microsoft.Web.WebView2.Core;
using SwashbucklerDiary.Extensions;
using SwashbucklerDiary.Utilities;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;

namespace SwashbucklerDiary
{
    public partial class MainPage
    {
        private partial void BlazorWebViewInitializing(object? sender, BlazorWebViewInitializingEventArgs e)
        {
            e.EnvironmentOptions = new()
            {
                //禁用自动播放，但不知道为什么没有生效
                AdditionalBrowserArguments = "--autoplay-policy=user-gesture-required",
            };
        }

        private partial void BlazorWebViewInitialized(object? sender, BlazorWebViewInitializedEventArgs e)
        {
            var webview2 = e.WebView.CoreWebView2;
            webview2.WebResourceRequested += WebView2WebResourceRequested;
        }

        async void WebView2WebResourceRequested(CoreWebView2 webview2, CoreWebView2WebResourceRequestedEventArgs args)
        {
            var intercept = await InterceptCustomPathRequest(webview2, args);
            if (intercept)
            {
                return;
            }
        }

        static async Task<bool> InterceptCustomPathRequest(CoreWebView2 webview2, CoreWebView2WebResourceRequestedEventArgs args)
        {
            string uri = args.Request.Uri;
            if (!uri.StartsWith(StaticCustomPath.InterceptPrefix))
            {
                return false;
            }

            var path = uri.TrimStart(StaticCustomPath.InterceptPrefix);
            path = Path.Combine(path.Split("/"));
            var filePath = Path.Combine(FileSystem.AppDataDirectory, path);
            if (File.Exists(filePath))
            {
                args.Response = await CreateWebResourceResponse(webview2, args, filePath);
            }
            else
            {
                args.Response = webview2.Environment.CreateWebResourceResponse(null, 404, "Not Found", string.Empty);
            }

            return true;

            static string GetHeaderString(IDictionary<string, string> headers) =>
                string.Join(Environment.NewLine, headers.Select(kvp => $"{kvp.Key}: {kvp.Value}"));

            static async Task<CoreWebView2WebResourceResponse> CreateWebResourceResponse(CoreWebView2 webview2, CoreWebView2WebResourceRequestedEventArgs args, string filePath)
            {
                var contentType = StaticContentProvider.GetResponseContentTypeOrDefault(filePath);
                var headers = StaticContentProvider.GetResponseHeaders(contentType);
                using var contentStream = File.OpenRead(filePath);
                var length = contentStream.Length;
                long rangeStart = 0;
                long rangeEnd = length - 1;

                int StatusCode = 200;
                string ReasonPhrase = "OK";

                //适用于音频视频文件资源的响应
                bool partial = args.Request.Headers.Contains("Range");
                if (partial)
                {
                    StatusCode = 206;
                    ReasonPhrase = "Partial Content";

                    var rangeString = args.Request.Headers.GetHeader("Range");
                    var ranges = rangeString.Split('=');
                    if (ranges.Length > 1 && !string.IsNullOrEmpty(ranges[1]))
                    {
                        string[] rangeDatas = ranges[1].Split("-");
                        rangeStart = Convert.ToInt64(rangeDatas[0]);
                        if (rangeDatas.Length > 1 && !string.IsNullOrEmpty(rangeDatas[1]))
                        {
                            rangeEnd = Convert.ToInt64(rangeDatas[1]);
                        }
                        else
                        {
                            rangeEnd = Math.Min(rangeEnd, rangeStart + 4 * 1024 * 1024);
                        }
                    }

                    headers.Add("Accept-Ranges", "bytes");
                    headers.Add("Content-Range", $"bytes {rangeStart}-{rangeEnd}/{length}");
                }

                headers.Add("Content-Length", (rangeEnd - rangeStart + 1).ToString());
                var headerString = GetHeaderString(headers);
                IRandomAccessStream stream = await ReadStreamRange(contentStream, rangeStart, rangeEnd);
                return webview2.Environment.CreateWebResourceResponse(stream, StatusCode, ReasonPhrase, headerString);
            }

            static async Task<IRandomAccessStream> ReadStreamRange(Stream contentStream, long start, long end)
            {
                long length = end - start + 1;
                contentStream.Position = start;

                using var memoryStream = new MemoryStream();
                byte[] buffer = new byte[4 * 1024 * 1024];
                int bytesRead;

                while (length > 0 && (bytesRead = contentStream.Read(buffer, 0, (int)Math.Min(buffer.Length, length))) > 0)
                {
                    memoryStream.Write(buffer, 0, bytesRead);
                    length -= bytesRead;
                }

                // 将内存流的位置重置为起始位置
                memoryStream.Seek(0, SeekOrigin.Begin);
                var randomAccessStream = new InMemoryRandomAccessStream();
                await randomAccessStream.WriteAsync(memoryStream.GetWindowsRuntimeBuffer());
                return randomAccessStream;
            }
        }
    }
}
