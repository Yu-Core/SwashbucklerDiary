using Microsoft.Web.WebView2.Core;
using SwashbucklerDiary.Shared;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Storage.Streams;
using WebView2Control = Microsoft.UI.Xaml.Controls.WebView2;

namespace SwashbucklerDiary.Maui.BlazorWebView
{
    public partial class MauiBlazorWebViewHandler
    {
        protected override void ConnectHandler(WebView2Control platformView)
        {
            base.ConnectHandler(platformView);
            platformView.CoreWebView2Initialized += CoreWebView2Initialized;
        }

        protected override void DisconnectHandler(WebView2Control platformView)
        {
            platformView.CoreWebView2Initialized -= CoreWebView2Initialized;
            base.DisconnectHandler(platformView);
        }

        private void CoreWebView2Initialized(WebView2Control sender, Microsoft.UI.Xaml.Controls.CoreWebView2InitializedEventArgs args)
        {
            var webview2 = sender.CoreWebView2;
            webview2.WebResourceRequested += WebView2WebResourceRequested;
        }

        async void WebView2WebResourceRequested(CoreWebView2 webview2, CoreWebView2WebResourceRequestedEventArgs args)
        {
            await InterceptCustomPathRequest(webview2, args);
        }

        static async Task<bool> InterceptCustomPathRequest(CoreWebView2 webview2, CoreWebView2WebResourceRequestedEventArgs args)
        {
            string uri = args.Request.Uri;
            if (!InterceptLocalFileRequest(uri, out string filePath))
            {
                return false;
            }

            args.Response = await CreateLocalFileResponse(webview2, args, filePath);
            return true;

            static string GetHeaderString(IDictionary<string, string> headers) =>
                string.Join(Environment.NewLine, headers.Select(kvp => $"{kvp.Key}: {kvp.Value}"));

            static async Task<CoreWebView2WebResourceResponse> CreateLocalFileResponse(CoreWebView2 webview2, CoreWebView2WebResourceRequestedEventArgs args, string filePath)
            {
                var contentType = StaticContentProvider.GetResponseContentTypeOrDefault(filePath);
                var headers = StaticContentProvider.GetResponseHeaders(contentType);
                using var contentStream = File.OpenRead(filePath);
                var length = contentStream.Length;
                long rangeStart = 0;
                long rangeEnd = length - 1;

                int statusCode = 200;
                string reasonPhrase = "OK";

                //适用于音频视频文件资源的响应
                bool partial = args.Request.Headers.Contains("Range");
                if (partial)
                {
                    statusCode = 206;
                    reasonPhrase = "Partial Content";

                    var rangeString = args.Request.Headers.GetHeader("Range");
                    int rangeDatasLength = ParseRange(rangeString, ref rangeStart, ref rangeEnd);
                    if (rangeDatasLength == 1)
                    {
                        //每次加载4Mb，不能设置太多
                        rangeEnd = Math.Min(rangeEnd, rangeStart + 4 * 1024 * 1024);
                    }

                    headers.Add("Accept-Ranges", "bytes");
                    headers.Add("Content-Range", $"bytes {rangeStart}-{rangeEnd}/{length}");
                }

                headers.Add("Content-Length", (rangeEnd - rangeStart + 1).ToString());
                var headerString = GetHeaderString(headers);
                IRandomAccessStream stream = await ReadStreamRange(contentStream, rangeStart, rangeEnd);
                return webview2.Environment.CreateWebResourceResponse(stream, statusCode, reasonPhrase, headerString);
            }

            static async Task<IRandomAccessStream> ReadStreamRange(Stream contentStream, long start, long end)
            {
                long length = end - start + 1;
                contentStream.Position = start;

                using var memoryStream = new MemoryStream();

                StreamCopy(contentStream, memoryStream, length);
                // 将内存流的位置重置为起始位置
                memoryStream.Seek(0, SeekOrigin.Begin);

                var randomAccessStream = new InMemoryRandomAccessStream();
                await randomAccessStream.WriteAsync(memoryStream.GetWindowsRuntimeBuffer());

                return randomAccessStream;
            }

            // 辅助方法，用于限制StreamCopy复制的数据长度
            static void StreamCopy(Stream source, Stream destination, long length)
            {
                //缓冲区设为1Mb,应该是够了
                byte[] buffer = new byte[1024 * 1024];
                int bytesRead;

                while (length > 0 && (bytesRead = source.Read(buffer, 0, (int)Math.Min(buffer.Length, length))) > 0)
                {
                    destination.Write(buffer, 0, bytesRead);
                    length -= bytesRead;
                }
            }
        }
    }
}
