using EmbedIO;
using EmbedIO.Actions;
using EmbedIO.Utilities;
using SwashbucklerDiary.Rcl.Services;
using System.Net;

namespace SwashbucklerDiary.Rcl.Hybird.Services
{
    public class ProxyService : IProxyService, IDisposable
    {
        private WebServer? server;
        public string ProxyUrl { get; }

        public ProxyService()
        {
            var baseUrl = $"http://localhost:{IPAddress.Loopback.GetAvailablePort()}/";
            ProxyUrl = $"{baseUrl}?url=";
            StartWebServer(baseUrl);
        }

        private void StartWebServer(string url)
        {
            if (server is not null)
            {
                return;
            }

            server = new WebServer(o => o
                    .WithUrlPrefix(url))
                    .WithCors()
                    .WithModule(new ActionModule("/", HttpVerbs.Any, ProxyHandler));

            server.RunAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                server?.Dispose();
                server = null;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        private readonly HttpClient _httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        private async Task ProxyHandler(IHttpContext context)
        {
            var request = context.Request;

            // 获取目标URL（这里假设客户端通过 ?url=xxx 指定目标地址）
            var targetUrl = request.QueryString["url"];
            if (string.IsNullOrWhiteSpace(targetUrl))
            {
                context.Response.StatusCode = 400;
                await context.SendStringAsync("Missing target url", "text/plain", System.Text.Encoding.UTF8);
                return;
            }

            // 转发请求
            var proxyRequest = new HttpRequestMessage(new HttpMethod(request.HttpMethod), targetUrl);

            // 复制请求头
            foreach (string headerName in request.Headers)
            {
                if (!WebHeaderCollection.IsRestricted(headerName) && !IsUnsafeHeader(headerName))
                    proxyRequest.Headers.TryAddWithoutValidation(headerName, request.Headers[headerName]);
            }

            proxyRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36");

            // 如果有请求体，则转发
            if (request.ContentLength64 > 0)
            {
                using var reader = new StreamReader(request.InputStream);
                string body = await reader.ReadToEndAsync();
                proxyRequest.Content = new StringContent(body);
            }

            // 发送请求到目标服务器
            var responseMessage = await _httpClient.SendAsync(proxyRequest);

            // 设置返回头
            foreach (var header in responseMessage.Headers)
            {
                if (!context.Response.Headers.ContainsKey(header.Key))
                {
                    context.Response.Headers[header.Key] = string.Join(",", header.Value);
                }
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                if (!context.Response.Headers.ContainsKey(header.Key))
                {
                    context.Response.Headers[header.Key] = string.Join(",", header.Value);
                }
            }

            // 设置状态码
            context.Response.StatusCode = (int)responseMessage.StatusCode;

            // 设置MIME 类型
            var contentType = responseMessage.Content.Headers.ContentType?.ToString();
            if (contentType is not null)
            {
                context.Response.Headers.Remove("Content-Type");
                context.Response.ContentType = contentType;
            }

            // 写回响应内容
            using var responseStream = await responseMessage.Content.ReadAsStreamAsync();
            await responseStream.CopyToAsync(context.Response.OutputStream);
        }

        static readonly string[] unsafeHeaders =
        [
            "Host", "Connection", "Content-Length",
            "TE", "Upgrade", "Proxy-Connection",
            "Origin", "User-Agent"
        ];

        private static bool IsUnsafeHeader(string header)
        {
            foreach (var unsafeHeader in unsafeHeaders)
            {
                if (header.Equals(unsafeHeader, StringComparison.OrdinalIgnoreCase))
                    return true;
            }
            return false;
        }
    }
}
