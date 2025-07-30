using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class PageCacheContainer : MyComponentBase
    {
        private PPageContainerReplacement? pPageContainerReplacement;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        private void HandlePageCacheUpdate()
        {
            var patternPaths = NavigateController.PageCachePaths
                .Select(path => new PatternPath(path))
                .ToArray();
            pPageContainerReplacement?.UpdatePatternPaths(patternPaths);
        }
    }
}
