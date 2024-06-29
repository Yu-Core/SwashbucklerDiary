using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class PageCacheContainer : ComponentBase, IDisposable
    {
        private bool pageUpdate = true;

        [Inject]
        private INavigateController NavigateController { get; set; } = default!;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            NavigateController.PageUpdateChanged += OnPageUpdateChanged;
        }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            builder.OpenComponent(0, typeof(PPageContainerReplacement));
            builder.AddComponentParameter(1, nameof(PPageContainerReplacement.UseRegex), false);
            builder.AddComponentParameter(2, nameof(PPageContainerReplacement.PageUpdate), pageUpdate);
            builder.AddComponentParameter(3, nameof(PPageContainerReplacement.IncludePatterns), NavigateController.PageCachePaths);
            builder.AddComponentParameter(4, nameof(PPageContainerReplacement.ChildContent), ChildContent);
            builder.CloseComponent();
        }

        private void OnPageUpdateChanged(object? sender, bool e)
        {
            pageUpdate = e;
        }

        public void Dispose()
        {
            NavigateController.PageUpdateChanged -= OnPageUpdateChanged;
            GC.SuppressFinalize(this);
        }
    }
}
