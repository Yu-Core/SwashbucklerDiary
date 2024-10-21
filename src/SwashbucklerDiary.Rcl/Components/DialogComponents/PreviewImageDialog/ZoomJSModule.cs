using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    internal class ZoomJSModule : JSModuleExtension
    {
        private readonly IPlatformIntegration _platformIntegration;

        public ZoomJSModule(IJSRuntime js, IPlatformIntegration platformIntegration) : base(js, "js/zoom-helper.js")
        {
            _platformIntegration = platformIntegration;
        }

        public async Task Init(string selector)
        {
            await base.InvokeVoidAsync("initZoom", selector);
        }

        public async Task Reset(string selector)
        {
            await base.InvokeVoidAsync("reset", selector);
        }

        public async Task ImgDragAndDrop(ElementReference element)
        {
            if (_platformIntegration.CurrentPlatform == AppDevicePlatform.Android || _platformIntegration.CurrentPlatform == AppDevicePlatform.iOS) return;

            await base.InvokeVoidAsync("imgDragAndDropForDesktop", element);
        }

        public ValueTask<string?> GetStyle(ElementReference element, string styleProp)
        {
            return base.InvokeAsync<string?>("getStyle", [element, styleProp]);
        }
    }
}
