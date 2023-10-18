using BlazorComponent.JSInterop;
using Microsoft.JSInterop;
using Serilog;
using Serilog.Core;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Utilities;

namespace SwashbucklerDiary.Services
{
    public class ScreenshotJSModule : JSModule
    {
        private readonly IAppDataService AppDataService;
        private readonly HttpClient HttpClient;

        public ScreenshotJSModule(IJSRuntime js, IAppDataService appDataService) : base(js, "./js/screenshot.js")
        {
            AppDataService = appDataService;
            HttpClient = new HttpClient();
        }

        public async Task<string> GetScreenshotBase64(string selector)
        {
            var dotNetCallbackRef = DotNetObjectReference.Create(this);

            return await InvokeAsync<string>("getScreenshotBase64", dotNetCallbackRef, nameof(UriToBase64), selector);
        }

        [JSInvokable]
        public async Task<string> UriToBase64(string uri)
        {
            try
            {
                if (StaticCustomScheme.IsInternalUri(uri))
                {
                    return uri;
                }
                else
                {
                    var bytes = await HttpClient.GetByteArrayAsync(uri);
                    return ConvertToBase64(uri, bytes);
                }
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}\n{e.StackTrace}");
                return string.Empty;
            }
        }

        public static string ConvertToBase64(string imagePath, byte[] imageBytes)
        {
            // 获取文件类型
            string fileType = Path.GetExtension(imagePath).ToLower();

            // 根据文件类型添加对应的data URI头部
            string base64String = $"data:image/{fileType.Substring(1)};base64,";

            // 将图片字节数组转换为base64字符串
            base64String += Convert.ToBase64String(imageBytes);

            return base64String;
        }

    }
}
