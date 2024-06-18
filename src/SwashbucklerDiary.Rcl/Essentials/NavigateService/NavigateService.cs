using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using System.Reflection;

namespace SwashbucklerDiary.Rcl.Essentials
{
    public abstract class NavigateService : NavigateActionController, INavigateService
    {
        private readonly RouteHelper routeHelper;

        public event Action<string>? PageCacheRemoved;

        public event Func<LocationChangingContext, Task>? LocationChanging;

        public List<string> PermanentPaths { get; set; } = [];

        public NavigateService()
        {
            routeHelper = new RouteHelper(Assemblies);
        }

        public async Task Init(NavigationManager navigationManager, IJSRuntime jSRuntime, IEnumerable<string> uris)
        {
            if (Initialized)
            {
                return;
            }

            PermanentPaths = uris.ToList();
            await base.Init(navigationManager, jSRuntime);
        }

        protected abstract Assembly[] Assemblies { get; }

        protected override async ValueTask OnLocationChanging(LocationChangingContext context)
        {
            var targetUri = _navigationManager.ToAbsoluteUri(context.TargetLocation).ToString();

            if (targetUri == stackBottomUri)
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
                return;
            }

            if (targetUri == _navigationManager.Uri)
            {
                context.PreventNavigation();
                return;
            }

            if (!routeHelper.IsMatch(targetUri))
            {
                context.PreventNavigation();
                return;
            }

            if (LocationChanging is not null)
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
