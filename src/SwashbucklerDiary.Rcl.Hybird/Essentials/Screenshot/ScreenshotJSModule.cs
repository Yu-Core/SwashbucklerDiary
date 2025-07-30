using Masa.Blazor.JSInterop;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Hybird.Essentials
{
    public class ScreenshotJSModule : JSModule
    {
        private readonly HttpClient _httpClient;

        private readonly ILogger<ScreenshotJSModule> _logger;

        private readonly DotNetObjectReference<object>? _dotNetObjectReference;

        public ScreenshotJSModule(IJSRuntime js,
            ILogger<ScreenshotJSModule> logger)
            : base(js, "./_content/SwashbucklerDiary.Rcl.Hybird/js/screenshot.js")
        {
            _logger = logger;
            _httpClient = new HttpClient();
            _dotNetObjectReference = DotNetObjectReference.Create<object>(this);
        }

        public async Task<Stream> GetScreenshotStream(string selector)
        {
            var dataReference = await InvokeAsync<IJSStreamReference>("getScreenshotStream", _dotNetObjectReference, nameof(HandleCorsUri), selector);
            return await dataReference.OpenReadStreamAsync(maxAllowedSize: 10_000_000);
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

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            _dotNetObjectReference?.Dispose();
        }

        private static string ConvertToBase64(string imagePath, byte[] imageBytes)
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
