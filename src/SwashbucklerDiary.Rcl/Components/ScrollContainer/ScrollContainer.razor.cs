using Masa.Blazor.Core;
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

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Id ??= $"scroll-container-{Guid.NewGuid():N}";
        }

        private string InternalClass => new CssBuilder()
            .Add("my-scroll-container")
            .Add(Class)
            .ToString();

        private string InternalContentClass => new CssBuilder()
            .Add("my-container")
            .Add(ContentClass)
            .ToString();
    }
}
