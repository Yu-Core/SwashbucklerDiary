using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class ScrollContainer
    {
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

        public ElementReference Ref { get; set; }
    }
}
