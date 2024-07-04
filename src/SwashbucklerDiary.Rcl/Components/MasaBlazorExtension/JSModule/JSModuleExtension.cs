using Masa.Blazor.JSInterop;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Components
{
    public class JSModuleExtension : JSModule
    {
        public JSModuleExtension(IJSRuntime js, string moduleUrl) : base(js, $"./_content/{StaticWebAssets.RclAssemblyName}/{moduleUrl}")
        {
        }
    }
}
