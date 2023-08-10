using Microsoft.JSInterop;

namespace SwashbucklerDiary.Utilities
{
    public static class StaticCustomScheme
    {
        //Windows暂时无法拦截自定义协议，所以需要将自定义协议渲染为https:// 
        //已经向WebView2提了这个问题 https://github.com/MicrosoftEdge/WebView2Feedback/issues/3658
        public static async Task CustomSchemeRender(this IJSRuntime js)
        {
            if (OperatingSystem.IsWindows())
            {
                await js.InvokeVoidAsync("HandleCustomSchemeRender", null);
            }
        }

        public static string CustomSchemeRender(string uri)
        {
            if (OperatingSystem.IsWindows())
            {
                uri = uri.Replace("appdata:///", "https://appdata/");
            }

            return uri;
        }
    }
}
