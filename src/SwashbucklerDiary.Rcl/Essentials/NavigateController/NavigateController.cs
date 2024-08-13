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
        protected const string stackBottomRelativePath = "stackBottom";

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

        protected object _lock = new();

        protected string? secondaryBackTargetUri;

        protected string? stackBottomPath;

        public event EventHandler<bool>? PageUpdateChanged;

        public bool IsInitialized { get; protected set; }

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

        public void Init(NavigationManager navigationManager, IJSRuntime jSRuntime, IEnumerable<string> paths, Assembly[] assemblies)
        {
            if (IsInitialized) return;

            lock (_lock)
            {
                if (!IsInitialized)
                {
                    InitCore(navigationManager, jSRuntime, paths, assemblies);
                    IsInitialized = true;
                }
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

        protected abstract Task HandleNavigateToStackBottomPath(LocationChangingContext context);

        private void InitCore(NavigationManager navigationManager, IJSRuntime jSRuntime, IEnumerable<string> paths, Assembly[] assemblies)
        {
            _navigationManager = navigationManager;
            _jSRuntime = jSRuntime;
            permanentPaths = paths.ToList();
            routeHelper = new RouteHelper(assemblies);
            // Insert a uri on the previous page. This can ensure that OnLocationChanging will definitely trigger
            var uri = _navigationManager.Uri;
            var absolutePath = new Uri(uri).AbsolutePath;
            var stackBottomUri = _navigationManager.ToAbsoluteUri(stackBottomRelativePath);
            stackBottomPath = stackBottomUri.AbsolutePath;

            //When the current page is not a 'stack bottom page', there is no need to replace it
            if (stackBottomPath != absolutePath)
            {
                _navigationManager.NavigateTo(stackBottomUri.ToString(), replace: true);
            }

            historyPaths.Add(stackBottomPath);

            if (!permanentPaths.Contains(absolutePath))
            {
                _navigationManager.NavigateTo("");
                historyPaths.Add(_navigationManager.ToAbsoluteUri("").AbsolutePath);
            }

            if (stackBottomPath != absolutePath)
            {
                _navigationManager.NavigateTo(uri);
                historyPaths.Add(absolutePath);
                AddPageCache(absolutePath);
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

            //路由不存在的页面禁止跳转
            var route = _navigationManager.ToRoute(targetUri);
            if (routeHelper is not null && !routeHelper.IsMatch(route))
            {
                context.PreventNavigation();
                return;
            }

            bool isPermanentPath = permanentPaths.Contains(targetPath);
            int currentHistoryIndex = historyPaths.Count - 1;
            int targetHistoryIndex = isPermanentPath ? 1 : (historyPaths.IndexOf(targetPath) is var index && index == -1 ? historyPaths.Count : index);

            if (currentHistoryIndex == targetHistoryIndex)
            {
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
            }
            //页面后退
            else if (currentHistoryIndex > targetHistoryIndex)
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
                        if (permanentPaths.Contains(currentPath))
                        {
                            await HandleNavigateToStackBottomPath(context);
                        }
                        else
                        {
                            context.PreventNavigation();
                            _navigationManager.NavigateTo("");
                        }
                    }
                    else
                    {
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
                            int startIndex = targetHistoryIndex + 1;
                            int count = currentHistoryIndex - targetHistoryIndex;
                            var clearPaths = historyPaths.GetRange(startIndex, count);
                            historyPaths.RemoveRange(startIndex, count);
                            pageCachePaths.RemoveAll(clearPaths.Contains);
                            historyActions.RemoveAll(it => !string.IsNullOrEmpty(it.Path) && clearPaths.Contains(it.Path));
                        }
                    }
                }

                waitNavigate = false;
            }
            //页面前进
            else
            {
                historyPaths.Add(targetPath);
                AddPageCache(targetPath);
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

        private void AddPageCache(string path)
        {
            if (!permanentPaths.Contains(path))
            {
                pageCachePaths.Add(path);
            }
        }
    }
}
