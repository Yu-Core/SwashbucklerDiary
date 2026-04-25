using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public class SwiperJsModule : CustomJSModule
    {
        public SwiperJsModule(IJSRuntime js) : base(js, "Components/Swiper/SwiperTabItems.razor.js")
        {
        }

        public async Task Init(DotNetObjectReference<object> dotNetObjectReference, ElementReference element, Dictionary<string, object> options)
        {
            await InvokeVoidAsync("init", [dotNetObjectReference, element, options]);
        }

        public async Task SlideToAsync(ElementReference element, int index)
        {
            await InvokeVoidAsync("slideTo", [element, index]);
        }

        public async Task DestroyAsync(ElementReference element)
        {
            await InvokeVoidAsync("destroy", element);
        }
    }
}
