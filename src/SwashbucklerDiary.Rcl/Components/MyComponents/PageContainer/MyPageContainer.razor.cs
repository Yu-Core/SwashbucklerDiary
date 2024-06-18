using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MyPageContainer : IDisposable
    {
        private readonly List<string> _includePatterns = [];

        private readonly List<string> _historyPaths = [];

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

            var currentPath = NavigationManager.GetAbsolutePath();
            if (_historyPaths.Count == 0 && !PermanentPaths.Contains(currentPath))
            {
                _historyPaths.Add(currentPath);
                _includePatterns.Add(currentPath);
            }

            NavigateService.LocationChanging += OnLocationChanging;
            NavigateService.PageCacheRemoved += OnPageCacheRemoved;
        }

        private List<string> IncludePatterns => _includePatterns.Union(PermanentPaths).ToList();

        private List<string> HistoryPaths => _historyPaths.Union(PermanentPaths).ToList();

        private void OnPageCacheRemoved(string url)
        {
            var absolutePath = new Uri(url).AbsolutePath;
            _includePatterns.Remove(absolutePath);
        }

        private Task OnLocationChanging(LocationChangingContext context)
        {
            var targetPath = NavigationManager.ToAbsoluteUri(context.TargetLocation).AbsolutePath;
            var currentPath = NavigationManager.GetAbsolutePath();
            if (targetPath == currentPath)
            {
                return Task.CompletedTask;
            }

            // 判断是后退还是前进
            if (HistoryPaths.Contains(targetPath))
            {
                _historyPaths.Remove(currentPath);
                _includePatterns.Remove(currentPath);
            }
            else
            {
                _historyPaths.Add(targetPath);
                _includePatterns.Add(targetPath);
            }

            //跳转PermanentPaths中的页面，清空其他页面
            if (PermanentPaths.Contains(targetPath))
            {
                _historyPaths.Clear();
                _includePatterns.Clear();
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