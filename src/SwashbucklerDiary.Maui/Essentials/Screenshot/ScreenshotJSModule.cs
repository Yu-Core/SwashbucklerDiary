using BlazorComponent.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using SwashbucklerDiary.Maui.Utilities;

namespace SwashbucklerDiary.Maui.Essentials
{
    public class ScreenshotJSModule : JSModule
    {
        private readonly string _baseUri;

        private readonly HttpClient _httpClient;

        private readonly ILogger<ScreenshotJSModule> _logger;

        public ScreenshotJSModule(IJSRuntime js,
            NavigationManager navigationManager,
            ILogger<ScreenshotJSModule> logger)
            : base(js, "./js/screenshot.js")
        {
            _baseUri = navigationManager.BaseUri;
            _logger = logger;
            _httpClient = new HttpClient();
        }

        public async Task<string> GetScreenshotBase64(string selector)
        {
            var dotNetCallbackRef = DotNetObjectReference.Create(this);
            return await InvokeAsync<string>("getScreenshotBase64", dotNetCallbackRef, nameof(HandleCorsUri), selector);
        }

        [JSInvokable]
        public async Task<string> HandleCorsUri(string uri)
        {
            try
            {
                var bytes = await _httpClient.GetByteArrayAsync(uri);
                return ConvertToBase64(uri, bytes);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"{nameof(HandleCorsUri)} fail");
            }

            return string.Empty;
        }

        public static string ConvertToBase64(string imagePath, byte[] imageBytes)
        {
            // 获取文件类型
            string fileType = StaticContentProvider.GetResponseContentTypeOrDefault(imagePath);

            // 根据文件类型添加对应的data URI头部
            string base64String = $"data:{fileType};base64,";

            // 将图片字节数组转换为base64字符串
            base64String += Convert.ToBase64String(imageBytes);

            return base64String;
        }

    }
}
