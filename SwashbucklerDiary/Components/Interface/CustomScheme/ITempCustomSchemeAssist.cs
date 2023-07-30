using Microsoft.JSInterop;

namespace SwashbucklerDiary.Components
{
    public interface ITempCustomSchemeAssist
    {
        public IJSRuntime JS { get; set; }
    }

    public static class ICustomSchemeExtend
    {
        //Windows暂时无法拦截自定义协议，所以需要将自定义协议渲染为https:// 
        //已经向WebView2提了这个问题 https://github.com/MicrosoftEdge/WebView2Feedback/issues/3658
        public static async Task CustomSchemeRender(this ITempCustomSchemeAssist component)
        {
            if (OperatingSystem.IsWindows())
            {
                await component.JS.InvokeVoidAsync("HandleCustomSchemeRender", null);
            }
        }

        public static string CustomSchemeRender(this ITempCustomSchemeAssist component,string uri)
        {
            if (OperatingSystem.IsWindows())
            {
                uri = uri.Replace("appdata:///", "appdata/");
            }

            return uri;
        }
    }
}
