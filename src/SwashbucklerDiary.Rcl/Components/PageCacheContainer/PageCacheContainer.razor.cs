using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class PageCacheContainer : MyComponentBase
    {
        private PPageContainerReplacement? pPageContainerReplacement;

        protected bool firstEnter = true;
        protected bool waitNavigate;
        protected bool allowPageUpdate;
        protected IDisposable? registration;
        protected List<PathString> historyPaths = [];
        protected HashSet<PathString> pageCachePaths = [];
        protected List<PathString> permanentPaths = [];
        protected List<PathString> NotUpdatePagePaths = [];
        protected readonly List<HistoryAction> historyActions = [];
        protected PathString? secondaryBackTargetUri;
        protected PathString? stackBottomPath;

        [Inject]
        private IAppLifecycle AppLifecycle { get; set; } = default!;

        [Inject]
        private IPlatformIntegration PlatformIntegration { get; set; } = default!;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter, EditorRequired]
        public IEnumerable<string> PermanentPaths { get; set; } = [];

        protected override void OnInitialized()
        {
            base.OnInitialized();

            NavigateController.OnHistoryActionAdded += AddHistoryAction;
            NavigateController.OnHistoryActionRemoved += RemoveHistoryAction;
            NavigateController.OnPageCacheRemoved += RemovePageCache;

            Init();
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            registration?.Dispose();
            NavigationManager.LocationChanged -= OnLocationChanged;
            NavigateController.OnHistoryActionAdded -= AddHistoryAction;
            NavigateController.OnHistoryActionRemoved -= RemoveHistoryAction;
            NavigateController.OnPageCacheRemoved -= RemovePageCache;
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            permanentPaths = PermanentPaths.Select(it => new PathString(it)).ToList();
        }

        private List<string> PageCachePaths => [.. permanentPaths.Union(pageCachePaths).Select(it => it.ToString())];

        private void HandlePageCacheUpdate()
        {
            var patternPaths = PageCachePaths
                .Select(path => new PatternPath(path))
                .ToArray();
            pPageContainerReplacement?.UpdatePatternPaths(patternPaths);
        }

        private void RemovePageCache(string url)
        {
            PathString absolutePath = new PathString(new Uri(url).AbsolutePath);
            pageCachePaths.Remove(absolutePath);
        }

        private void Init()
        {
            permanentPaths = PermanentPaths.Select(it => new PathString(it)).ToList();
            // Insert a uri on the previous page. This can ensure that OnLocationChanging will definitely trigger
            var uri = NavigationManager.Uri;
            PathString absolutePath = new PathString(new Uri(uri).AbsolutePath);
            var stackBottomUri = NavigationManager.ToAbsoluteUri(NavigateControllerHelper.StackBottomRelativePath);
            stackBottomPath = new PathString(stackBottomUri.AbsolutePath);

            allowPageUpdate = false;
            //When the current page is not a 'stack bottom page', there is no need to replace it
            if (stackBottomPath != absolutePath)
            {
                NavigationManager.NavigateTo(stackBottomUri.ToString(), replace: true);
                NotUpdatePagePaths.Add(stackBottomPath);
            }

            historyPaths.Add(stackBottomPath);

            if (!permanentPaths.Contains(absolutePath))
            {
                NavigationManager.NavigateTo("");
                PathString homePageAbsolutePath = new PathString(NavigationManager.ToAbsoluteUri("").AbsolutePath);
                historyPaths.Add(homePageAbsolutePath);
                NotUpdatePagePaths.Add(homePageAbsolutePath);
            }

            if (stackBottomPath != absolutePath)
            {
                NavigationManager.NavigateTo(uri);
                historyPaths.Add(absolutePath);
                AddPageCache(absolutePath);
                NotUpdatePagePaths.Add(absolutePath);
            }

            registration = NavigationManager.RegisterLocationChangingHandler(OnLocationChanging);
            NavigationManager.LocationChanged += OnLocationChanged;
        }

        private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            if (secondaryBackTargetUri is not null)
            {
                PathString uri = secondaryBackTargetUri;
                secondaryBackTargetUri = null;
                allowPageUpdate = true;
                if (uri != NavigationManager.Uri)
                {
                    Task.Run(() =>
                    {
                        NavigationManager.NavigateTo(uri.ToString());
                    });
                }
            }

            PathString currentPath = new(NavigationManager.GetAbsolutePath());
            if (NotUpdatePagePaths.Remove(currentPath) && NotUpdatePagePaths.Count == 0)
            {
                allowPageUpdate = true;
            }

        }

        private async ValueTask OnLocationChanging(LocationChangingContext context)
        {
            //不处理外部页面，按理说应该不会触发
            if (!NavigationManager.IsBaseOf(context.TargetLocation))
            {
                return;
            }

            if (waitNavigate)
            {
                context.PreventNavigation();
                return;
            }

            var targetUri = NavigationManager.ToAbsoluteUri(context.TargetLocation).ToString();
            PathString targetPath = new PathString(new Uri(targetUri).AbsolutePath);
            var currentUri = NavigationManager.Uri;
            var currentPath = new PathString(new Uri(NavigationManager.Uri).AbsolutePath);

            //路由不存在的页面禁止跳转
            var route = NavigationManager.ToRoute(targetUri);
            if (!NavigateController.RouteMatcher.IsMatch(route))
            {
                context.PreventNavigation();
                return;
            }

            bool isPermanentPath = permanentPaths.Contains(targetPath);
            int currentHistoryIndex = historyPaths.Count - 1;
            int targetHistoryIndex = isPermanentPath ? 1 : (historyPaths.IndexOf(targetPath) is var index && index == -1 ? historyPaths.Count : index);

            if (currentHistoryIndex == targetHistoryIndex)
            {
                if (NavigateController.DisableNavigate)
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
                            NavigationManager.NavigateTo(targetUri, replace: true);
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
                        NavigationManager.NavigateTo(targetUri, replace: true);
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
                if (NavigateController.DisableNavigate)
                {
                    AppLifecycle.QuitApp();
                    context.PreventNavigation();
                    return;
                }

                // 触发返回事件
                waitNavigate = true;
                bool backPrevious = currentHistoryIndex - targetHistoryIndex == 1;
                if (backPrevious && await HandleNavigateActionAsync())
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
                            NavigationManager.NavigateTo("");
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
                                allowPageUpdate = false;
                            }

                            await JS.HistoryGo(targetHistoryIndex - currentHistoryIndex);
                        }
                        // 二次返回
                        else
                        {
                            firstEnter = true;
                            int startIndex = targetHistoryIndex + 1;
                            int count = currentHistoryIndex - targetHistoryIndex;
                            var clearPaths = historyPaths.GetRange(startIndex, count);
                            historyPaths.RemoveRange(startIndex, count);
                            pageCachePaths.RemoveWhere(clearPaths.Contains);
                            AddPageCache(targetPath);
                            historyActions.RemoveAll(it => !string.IsNullOrEmpty(it.Path) && clearPaths.Contains(it.Path));
                        }
                    }
                }

                waitNavigate = false;
            }
            //页面前进
            else
            {
                if (NavigateController.DisableNavigate)
                {
                    context.PreventNavigation();
                    return;
                }

                historyActions.Where(it => it.IsDialog).ToList().ForEach(historyAction =>
                {
                    historyAction.Action?.Invoke();
                });

                if (context.HistoryEntryState == "replace")
                {
                    historyPaths.Remove(currentPath);
                    pageCachePaths.Remove(currentPath);
                }

                historyPaths.Add(targetPath);
                AddPageCache(targetPath);
            }
        }

        private async ValueTask<bool> HandleNavigateActionAsync()
        {
            var path = NavigationManager.GetAbsolutePath();
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

        protected async Task HandleNavigateToStackBottomPath(LocationChangingContext context)
        {
            if (PlatformIntegration.CurrentPlatform != Shared.AppDevicePlatform.Browser)
            {
                context.PreventNavigation();
                AppLifecycle.QuitApp();
            }
            else
            {
                // 首次返回
                if (firstEnter)
                {
                    firstEnter = false;
                    context.PreventNavigation();
                    await JS.HistoryBack();
                }
                // 二次返回
                else
                {
                    firstEnter = true;
                    historyPaths.RemoveAt(historyPaths.Count - 1);
                }
            }
        }

        private void AddPageCache(PathString path)
        {
            if (!permanentPaths.Contains(path))
            {
                pageCachePaths.Add(path);
            }
        }

        private void AddHistoryAction(HistoryAction historyAction)
        {
            historyAction.Path = NavigationManager.GetAbsolutePath();
            historyActions.Add(historyAction);
        }

        private void RemoveHistoryAction(Func<HistoryAction, bool> func)
        {
            var index = historyActions.FindIndex(func.Invoke);
            if (index >= 0)
            {
                historyActions.RemoveAt(index);
            }
        }
    }
}
