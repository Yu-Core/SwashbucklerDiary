using Masa.Blazor.JSInterop;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Components
{
    public class CustomJSModule : JSModule
    {
        public CustomJSModule(IJSRuntime js, string moduleUrl) : base(js, $"./_content/{StaticWebAssets.RclAssemblyName}/{moduleUrl}")
        {
        }
    }
}
