using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Extensions;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class PageCacheContainer : MyComponentBase
    {
        private CustomPPageContainer? pPageContainer;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            NavigateController.PageCachePathsChanged += HandlePageCacheRemoved;
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            NavigateController.PageCachePathsChanged -= HandlePageCacheRemoved;
        }

        private void HandlePageCacheRemoved(List<string> paths)
        {
            pPageContainer?.UpdatePatternPaths(paths);
        }
    }
}
