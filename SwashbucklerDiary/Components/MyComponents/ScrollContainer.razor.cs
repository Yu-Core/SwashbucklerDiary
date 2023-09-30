using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Components
{
    public partial class ScrollContainer
    {
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

        public ElementReference Ref { get; set; }
    }
}
