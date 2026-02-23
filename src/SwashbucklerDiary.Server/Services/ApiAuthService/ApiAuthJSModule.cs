using Masa.Blazor.JSInterop;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Server.Services
{
    public class ApiAuthJSModule : JSModule
    {
        public ApiAuthJSModule(IJSRuntime js) : base(js, "./js/apiAuth.js")
        {

        }

        public ValueTask SetCookie(string apiKey)
        {
            return InvokeVoidAsync("setCookie", apiKey);
        }
    }
}
