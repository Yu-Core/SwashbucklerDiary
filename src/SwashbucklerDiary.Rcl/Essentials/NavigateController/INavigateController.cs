using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Essentials
{
    public interface INavigateController
    {
        event Action<List<string>>? PageCachePathsChanged;

        bool IsInitialized { get; }

        bool AllowPageUpdate { get; }

        bool DisableNavigate { get; set; }

        RouteMatcher RouteMatcher { get; }

        void Init(NavigationManager navigationManager, IJSRuntime jSRuntime, IEnumerable<string> permanentPaths);

        void RemovePageCache(string url);

        void AddHistoryAction(Action action, bool preventNavigation = true);

        void AddHistoryAction(Func<Task> func, bool preventNavigation = true);

        void RemoveHistoryAction(Action action);

        void RemoveHistoryAction(Func<Task> func);
    }
}
