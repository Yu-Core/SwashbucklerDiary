using BlazorComponent.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Components
{
    public partial class ScrollContainer
    {
        private ElementReference element;
        [Inject]
        private IJSRuntime JS { get; set; } = default!;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }
        [Parameter]
        public string? Id { get; set; }
        [Parameter]
        public string? Class { get; set; }
        [Parameter]
        public string? Style { get; set; }
        [Parameter]
        public string? ContentClass { get; set; }
        [Parameter]
        public string? ContentStyle { get; set; }
        [Parameter]
        public EventCallback OnContextmenu { get; set; }

        public async Task ScrollToTop()
        {
            //直接滚动显得很生硬，所以延时0.2s
            await Task.Delay(200);
            await JS.ScrollTo(element, 0);
        }
    }
}
