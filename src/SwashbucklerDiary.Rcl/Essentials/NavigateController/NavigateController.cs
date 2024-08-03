using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Extensions;
using System.Reflection;

namespace SwashbucklerDiary.Rcl.Essentials
{
    public abstract class NavigateController : INavigateController, IDisposable
    {
        protected bool firstEnter = true;

        protected bool waitNavigate;

        protected IDisposable? registration;

        protected IJSRuntime _jSRuntime = default!;

        protected List<string> historyPaths = [];

        protected List<string> pageCachePaths = [];

        protected List<string> permanentPaths = [];

        protected NavigationManager _navigationManager = default!;

        protected readonly List<HistoryAction> historyActions = [];

        protected RouteHelper? routeHelper;

        protected SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 1);

        protected string? secondaryBackTargetUri;

        protected string? stackBottomPath;

        public event EventHandler<bool>? PageUpdateChanged;

        public bool Initialized { get; protected set; }

        public List<string> PageCachePaths => permanentPaths.Union(pageCachePaths).ToList();

        public void AddHistoryAction(Action action, bool preventNavigation = true)
            => AddHistoryAction(action, null, preventNavigation);

        public void AddHistoryAction(Func<Task> func, bool preventNavigation = true)
            => AddHistoryAction(null, func, preventNavigation);

        public void Dispose()
        {
            registration?.Dispose();
            _navigationManager.LocationChanged -= OnLocationChanged;
            GC.SuppressFinalize(this);
        }

        public async Task InitAsync(NavigationManager navigationManager, IJSRuntime jSRuntime, IEnumerable<string> paths, Assembly[] assemblies)
        {
            // 等待并请求进入
            await _semaphoreSlim.WaitAsync();

            try
            {
                if (Initialized)
                {
                    return;
                }

                await InitCoreAsync(navigationManager, jSRuntime, paths, assemblies);
                Initialized = true;
            }
            finally
            {
                _semaphoreSlim.Release();
            }
        }

        public void RemoveHistoryAction(Action action)
            => RemoveHistoryAction(it => it.Action == action);

        public void RemoveHistoryAction(Func<Task> func)
            => RemoveHistoryAction(it => it.Func == func);

        public void RemovePageCache(string url)
        {
            var absolutePath = new Uri(url).AbsolutePath;
            pageCachePaths.Remove(absolutePath);
        }

        protected abstract Task HandleNavigateToStackBottomPath();

        private async Task InitCoreAsync(NavigationManager navigationManager, IJSRuntime jSRuntime, IEnumerable<string> paths, Assembly[] assemblies)
        {
            _navigationManager = navigationManager;
            _jSRuntime = jSRuntime;
            permanentPaths = paths.ToList();
            routeHelper = new RouteHelper(assemblies);
            // Insert a uri on the previous page. This can ensure that OnLocationChanging will definitely trigger
            var uri = _navigationManager.Uri;
            var stackBottomUri = _navigationManager.ToAbsoluteUri(Guid.NewGuid().ToString("N"));
            stackBottomPath = stackBottomUri.AbsolutePath;
            await _jSRuntime.HistoryReplaceState(stackBottomUri.ToString());
            historyPaths.Add(stackBottomPath);

            var absolutePath = new Uri(uri).AbsolutePath;
            if (!permanentPaths.Contains(absolutePath))
            {
                _navigationManager.NavigateTo("");
                historyPaths.Add(_navigationManager.ToAbsoluteUri("").AbsolutePath);
            }

            _navigationManager.NavigateTo(uri);
            historyPaths.Add(absolutePath);
            if (!permanentPaths.Contains(absolutePath))
            {
                pageCachePaths.Add(absolutePath);
            }

            registration = _navigationManager.RegisterLocationChangingHandler(OnLocationChanging);
            _navigationManager.LocationChanged += OnLocationChanged;
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(secondaryBackTargetUri))
            {
                var uri = secondaryBackTargetUri;
                secondaryBackTargetUri = null;
                PageUpdateChanged?.Invoke(this, true);
                if (uri != _navigationManager.Uri)
                {
                    _navigationManager.NavigateTo(uri);
                }
            }
        }

        private async ValueTask OnLocationChanging(LocationChangingContext context)
        {
            //不处理外部页面，按理说应该不会触发
            if (!_navigationManager.IsBaseOf(context.TargetLocation))
            {
                return;
            }

            if (waitNavigate)
            {
                context.PreventNavigation();
                return;
            }

            var targetUri = _navigationManager.ToAbsoluteUri(context.TargetLocation).ToString();
            var targetPath = new Uri(targetUri).AbsolutePath;
            var currentUri = _navigationManager.Uri;
            var currentPath = new Uri(_navigationManager.Uri).AbsolutePath;

            var route = _navigationManager.ToRoute(targetUri);
            //路由不存在的页面禁止跳转
            if (targetPath != stackBottomPath && routeHelper is not null && !routeHelper.IsMatch(route))
            {
                context.PreventNavigation();
                return;
            }

            //路径不变
            if (targetPath == currentPath)
            {
                //更改查询参数，或更改hash
                if (currentUri == targetUri)
                {
                    context.PreventNavigation();
                }
                else
                {
                    if (firstEnter)
                    {
                        firstEnter = false;
                        context.PreventNavigation();
                        _navigationManager.NavigateTo(targetUri, replace: true);
                    }
                    else
                    {
                        firstEnter = true;
                    }
                }
            }
            //从常驻页面跳转常驻页面
            else if (permanentPaths.Contains(targetPath) && permanentPaths.Contains(currentPath))
            {
                if (firstEnter)
                {
                    firstEnter = false;
                    context.PreventNavigation();
                    _navigationManager.NavigateTo(targetUri, replace: true);
                }
                else
                {
                    firstEnter = true;
                    historyPaths.Remove(currentPath);
                    historyPaths.Add(targetPath);
                }
            }
            //判断页面后退
            else if (historyPaths.Contains(targetPath) || permanentPaths.Contains(targetPath))
            {
                // 触发返回事件
                waitNavigate = true;
                var preventNavigation = await HandleNavigateActionAsync();
                if (preventNavigation)
                {
                    context.PreventNavigation();
                }
                else
                {
                    // 处理跳转到栈底页面
                    if (targetPath == stackBottomPath)
                    {
                        context.PreventNavigation();
                        if (permanentPaths.Contains(currentPath))
                        {
                            await HandleNavigateToStackBottomPath();
                        }
                        else
                        {
                            _navigationManager.NavigateTo("");
                        }
                    }
                    else
                    {
                        bool isPermanentPath = permanentPaths.Contains(targetPath);
                        int currentHistoryIndex = historyPaths.Count - 1;
                        int targetHistoryIndex = isPermanentPath ? 1 : historyPaths.IndexOf(targetPath);
                        // 首次返回
                        if (firstEnter)
                        {
                            firstEnter = false;
                            context.PreventNavigation();
                            secondaryBackTargetUri = targetUri;
                            if (!historyPaths.Contains(targetPath) && isPermanentPath)
                            {
                                PageUpdateChanged?.Invoke(this, false);
                            }

                            await _jSRuntime.HistoryGo(targetHistoryIndex - currentHistoryIndex);
                        }
                        // 二次返回
                        else
                        {
                            firstEnter = true;
                            int index = targetHistoryIndex + 1;
                            int count = currentHistoryIndex - targetHistoryIndex;
                            var clearPaths = historyPaths.GetRange(index, count);
                            historyPaths.RemoveRange(index, count);
                            pageCachePaths.RemoveAll(clearPaths.Contains);
                            historyActions.RemoveAll(it => !string.IsNullOrEmpty(it.Path) && clearPaths.Contains(it.Path));
                        }
                    }
                }

                waitNavigate = false;
            }
            else
            {
                historyPaths.Add(targetPath);
                pageCachePaths.Add(targetPath);
            }
        }

        private void AddHistoryAction(Action? action = null, Func<Task>? func = null, bool preventNavigation = true)
        {
            var path = _navigationManager.GetAbsolutePath();
            historyActions.Add(new()
            {
                Path = path,
                PreventNavigation = preventNavigation,
                Action = action,
                Func = func
            });
        }

        private void RemoveHistoryAction(Func<HistoryAction, bool> func)
        {
            var index = historyActions.FindIndex(func.Invoke);
            if (index >= 0)
            {
                historyActions.RemoveAt(index);
            }
        }

        private async ValueTask<bool> HandleNavigateActionAsync()
        {
            var path = _navigationManager.GetAbsolutePath();
            var historyAction = historyActions.LastOrDefault(it => it.Path == path);
            if (historyAction is null)
            {
                return false;
            }

            historyAction.Action?.Invoke();
            if (historyAction.Func is not null)
            {
                await historyAction.Func.Invoke();
            }

            return historyAction.PreventNavigation;
        }
    }
}
