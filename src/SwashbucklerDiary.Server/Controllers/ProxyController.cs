using Microsoft.AspNetCore.Mvc;
using SwashbucklerDiary.Server.Attributes;
using System.Net;

namespace SwashbucklerDiary.Server.Controllers
{
    [ApiAuth]
    [ApiController]
    [Route("api/proxy")]
    public class ProxyController : ControllerBase
    {
        private readonly HttpClient _httpClient = new HttpClient()
        {
            Timeout = TimeSpan.FromSeconds(30)
        };

        [HttpGet]
        public async Task Proxy()
        {
            var request = Request;
            var response = Response;

            var targetUrl = request.Query["url"].ToString();
            if (string.IsNullOrWhiteSpace(targetUrl))
            {
                response.StatusCode = 400;
                await response.WriteAsync("Missing target url", System.Text.Encoding.UTF8);
                return;
            }

            // 转发请求
            var proxyRequest = new HttpRequestMessage(new HttpMethod(request.Method), targetUrl);

            // 复制请求头
            foreach ((var headerName, var headerValue) in request.Headers)
            {
                if (!WebHeaderCollection.IsRestricted(headerName) && !IsUnsafeHeader(headerName))
                    proxyRequest.Headers.TryAddWithoutValidation(headerName, headerValue.ToString());
            }

            // 修改请求头
            proxyRequest.Headers.Remove("User-Agent");
            proxyRequest.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/140.0.0.0 Safari/537.36");

            // 如果有请求体，则转发
            if (request.ContentLength > 0)
            {
                proxyRequest.Content = new StreamContent(Request.Body);
            }

            // 发送请求到目标服务器
            var responseMessage = await _httpClient.SendAsync(proxyRequest);

            // 设置返回头
            foreach (var header in responseMessage.Headers)
            {
                if (!response.Headers.ContainsKey(header.Key))
                {
                    response.Headers[header.Key] = header.Value.ToArray();
                }
            }

            foreach (var header in responseMessage.Content.Headers)
            {
                if (!response.Headers.ContainsKey(header.Key))
                {
                    response.Headers[header.Key] = header.Value.ToArray();
                }
            }

            // 设置状态码
            response.StatusCode = (int)responseMessage.StatusCode;

            if (response.StatusCode >= 200 &&
                response.StatusCode != 204 &&
                response.StatusCode != 304)
            {
                response.Headers.TransferEncoding = "";
                // 写回响应内容
                await responseMessage.Content.CopyToAsync(response.Body);
            }
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
