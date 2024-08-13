using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class PageCacheContainer : ComponentBase
    {
        [Inject]
        private INavigateController NavigateController { get; set; } = default!;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent(0, typeof(PPageContainerReplacement));
            builder.AddComponentParameter(1, nameof(PPageContainerReplacement.UseRegex), false);
            builder.AddComponentParameter(2, nameof(PPageContainerReplacement.PageUpdate), NavigateController.CanPageUpdate);
            builder.AddComponentParameter(3, nameof(PPageContainerReplacement.IncludePatterns), NavigateController.PageCachePaths);
            builder.AddComponentParameter(4, nameof(PPageContainerReplacement.ChildContent), ChildContent);
            builder.CloseComponent();
        }
    }
}
