using Microsoft.JSInterop;

namespace SwashbucklerDiary.Utilities
{
    //Windows暂时无法拦截自定义协议，所以需要将自定义协议渲染为https:// 
    //已经向WebView2提了这个问题 https://github.com/MicrosoftEdge/WebView2Feedback/issues/3658
    public static class StaticCustomScheme
    {
#if WINDOWS
        private readonly static string HttpsScheme = "https://appdata/";
        private readonly static string CustomScheme = "appdata:///";
#endif
        public static string ReverseCustomSchemeRender(string uri)
        {
#if WINDOWS
            uri = uri.Replace(HttpsScheme, CustomScheme);
#endif

            return uri;
        }

        public static string CustomSchemeRender(string uri)
        {
#if WINDOWS
            uri = uri.Replace(CustomScheme, HttpsScheme);
#endif

            return uri;
        }
    }
}
