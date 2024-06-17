using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Essentials
{
    public abstract class NavigateService : NavigateActionController, INavigateService
    {
        public event Action<string>? PageCacheRemoved;

        public event Func<LocationChangingContext, Task>? LocationChanging;

        public List<string> PermanentPaths { get; set; } = [];

        public async Task Init(NavigationManager navigationManager, IJSRuntime jSRuntime, IEnumerable<string> uris)
        {
            if (Initialized)
            {
                return;
            }

            PermanentPaths = uris.ToList();
            await base.Init(navigationManager, jSRuntime);
        }

        protected override async ValueTask OnLocationChanging(LocationChangingContext context)
        {
            if (stackBottomUri == _navigationManager.ToAbsoluteUri(context.TargetLocation).ToString())
            {
                if (PermanentPaths.Any(it => it == _navigationManager.GetAbsolutePath()))
                {
                    await HandleNavigateToStackBottomUri();
                }
                else
                {
                    _navigationManager.NavigateTo("", replace: true);
                }

                context.PreventNavigation();
            }
            else if (LocationChanging is not null)
            {
                await LocationChanging.Invoke(context);
            }
        }

        protected abstract override Task HandleNavigateToStackBottomUri();

        public void RemovePageCache(string url)
        {
            PageCacheRemoved?.Invoke(url);
        }
    }
}
