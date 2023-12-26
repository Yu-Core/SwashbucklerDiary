using BlazorComponent.JSInterop;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Component
{
    public class JSModuleExtension : JSModule
    {
        public JSModuleExtension(IJSRuntime js, string moduleUrl) : base(js, $"./_content/{StaticWebAssets.RclAssemblyName}/{moduleUrl}")
        {
        }
    }
}
