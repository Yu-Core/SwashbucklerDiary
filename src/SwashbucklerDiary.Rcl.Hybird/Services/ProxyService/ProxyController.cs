using EmbedIO;
using EmbedIO.Routing;
using EmbedIO.Utilities;
using EmbedIO.WebApi;
using System.Net;
using System.Net.Mime;

namespace SwashbucklerDiary.Rcl.Hybird.Services
{
    public class ProxyController : WebApiController
    {
        private readonly HttpClient _httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        [Route(HttpVerbs.Get, "/api/proxy")]
        public async Task Proxy()
        {
            var request = Request;
            var response = Response;

            // 获取目标URL（这里假设客户端通过 ?url=xxx 指定目标地址）
            var targetUrl = request.QueryString["url"];
            if (string.IsNullOrWhiteSpace(targetUrl))
            {
                response.StatusCode = 400;
                await HttpContext.SendStringAsync("Missing target url", "text/plain", System.Text.Encoding.UTF8);
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

            // 修改请求头
            proxyRequest.Headers.Remove("User-Agent");
            proxyRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36");

            // 如果有请求体，则转发
            if (request.ContentLength64 > 0)
            {
                proxyRequest.Content = new StreamContent(request.InputStream);
            }

            // 发送请求到目标服务器
            var responseMessage = await _httpClient.SendAsync(proxyRequest);

            // 设置返回头
            foreach (var header in responseMessage.Headers)
            {
                if (!response.Headers.ContainsKey(header.Key))
                {
                    response.Headers[header.Key] = string.Join(",", header.Value);
                }
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                if (!response.Headers.ContainsKey(header.Key))
                {
                    response.Headers[header.Key] = string.Join(",", header.Value);
                }
            }

            // 设置状态码
            response.StatusCode = (int)responseMessage.StatusCode;

            if (OperatingSystem.IsLinux())
            {
                response.Headers.Remove("Transfer-Encoding");
            }

            // iOS or MacCatalyst
            if (OperatingSystem.IsIOS())
            {
                response.Headers.Remove("Content-Encoding");
            }
            // 写回响应内容
            await responseMessage.Content.CopyToAsync(response.OutputStream);
        }

        static readonly string[] unsafeHeaders =
        [
            "Host", "Connection", "Content-Length",
            "TE", "Upgrade", "Proxy-Connection",
            "Origin"
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
