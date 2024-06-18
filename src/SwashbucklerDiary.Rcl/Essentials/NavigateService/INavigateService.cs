using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Essentials
{
    public interface INavigateService : INavigateActionController
    {
        event Action<string>? PageCacheRemoved;

        event Func<LocationChangingContext, Task>? LocationChanging;

        Task Init(NavigationManager navigationManager, IJSRuntime jSRuntime, IEnumerable<string> uris);

        void RemovePageCache(string url);
    }
}
