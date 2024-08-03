using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reflection;

namespace SwashbucklerDiary.Rcl.Essentials
{
    public interface INavigateController
    {
        bool Initialized { get; }

        List<string> PageCachePaths { get; }

        event EventHandler<bool> PageUpdateChanged;

        Task InitAsync(NavigationManager navigationManager, IJSRuntime jSRuntime, IEnumerable<string> uris, Assembly[] assemblies);

        void RemovePageCache(string url);

        void AddHistoryAction(Action action, bool preventNavigation = true);

        void AddHistoryAction(Func<Task> func, bool preventNavigation = true);

        void RemoveHistoryAction(Action action);

        void RemoveHistoryAction(Func<Task> func);
    }
}
