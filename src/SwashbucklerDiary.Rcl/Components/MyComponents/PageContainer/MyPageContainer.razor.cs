using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MyPageContainer : IDisposable
    {
        private List<string> includePatterns = [];

        [Inject]
        private INavigateService NavigateService { get; set; } = default!;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public IEnumerable<string> PermanentPaths { get; set; } = [];

        protected override void OnInitialized()
        {
            base.OnInitialized();

            NavigateService.LocationChanging += OnLocationChanging;
            NavigateService.PageCacheRemoved += OnPageCacheRemoved;
        }

        private void OnPageCacheRemoved(string url)
        {
            var absolutePath = new Uri(url).AbsolutePath;
            includePatterns.Remove(absolutePath);
        }

        private Task OnLocationChanging(LocationChangingContext context)
        {
            var targetPath = NavigationManager.ToAbsoluteUri(context.TargetLocation).AbsolutePath;
            var currentPath = new Uri(NavigationManager.Uri).AbsolutePath;
            if (targetPath == currentPath)
            {
                return Task.CompletedTask;
            }

            if (includePatterns.Count == 0)
            {
                includePatterns = PermanentPaths.ToList();
            }

            if (!includePatterns.Contains(targetPath))
            {
                includePatterns.Add(targetPath);
            }
            else
            {
                if (!PermanentPaths.Contains(currentPath))
                {
                    includePatterns.Remove(currentPath);
                }
            }

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            NavigateService.LocationChanging -= OnLocationChanging;
            NavigateService.PageCacheRemoved -= OnPageCacheRemoved;
            GC.SuppressFinalize(this);
        }
    }
}