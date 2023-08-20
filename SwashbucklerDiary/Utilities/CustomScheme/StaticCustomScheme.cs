using Microsoft.JSInterop;

namespace SwashbucklerDiary.Utilities
{
    public static class StaticCustomScheme
    {
#if WINDOWS
        //Windows暂时无法拦截自定义协议，所以需要将自定义协议渲染为https:// 
        //已经向WebView2提了这个问题 https://github.com/MicrosoftEdge/WebView2Feedback/issues/3658
        public static async Task CustomSchemeRender(this IJSRuntime js)
        {
            await js.InvokeVoidAsync("HandleCustomSchemeRender", null);
        }
#endif

        public static string CustomSchemeRender(string uri)
        {
#if WINDOWS
            uri = uri.Replace("appdata:///", "https://appdata/");
#endif

            return uri;
        }
    }
}
