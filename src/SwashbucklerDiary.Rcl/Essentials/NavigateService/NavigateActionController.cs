using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Extensions;

namespace SwashbucklerDiary.Rcl.Essentials
{
    public class NavigateActionController : INavigateActionController, IDisposable
    {
        protected string? stackBottomUri;

        protected IDisposable? registration;

        protected NavigationManager _navigationManager = default!;

        protected IJSRuntime _jSRuntime = default!;

        protected readonly List<HistoryAction> historyActions = [];

        public bool Initialized { get; protected set; }

        public async Task Init(NavigationManager navigationManager, IJSRuntime jSRuntime)
        {
            if (Initialized)
            {
                return;
            }

            Initialized = true;
            _navigationManager = navigationManager;
            _jSRuntime = jSRuntime;
            // Insert a uri on the previous page. This can ensure that OnLocationChanging will definitely trigger
            var uri = _navigationManager.Uri;
            stackBottomUri = new Uri(new Uri(_navigationManager.BaseUri), Guid.NewGuid().ToString()).ToString();
            await _jSRuntime.HistoryReplaceState(stackBottomUri);
            _navigationManager.NavigateTo(uri);
            registration = _navigationManager.RegisterLocationChangingHandler(OnLocationChangingCore);
        }

        public void Dispose() => registration?.Dispose();

        public void AddHistoryAction(Action action, bool preventNavigation = true)
            => AddHistoryAction(action, null, preventNavigation);

        public void AddHistoryAction(Func<Task> func, bool preventNavigation = true)
            => AddHistoryAction(null, func, preventNavigation);

        public void RemoveHistoryAction(Action action)
            => RemoveHistoryAction(it => it.Action == action);

        public void RemoveHistoryAction(Func<Task> func)
            => RemoveHistoryAction(it => it.Func == func);

        private async ValueTask OnLocationChangingCore(LocationChangingContext context)
        {
            var preventNavigation = await HandleNavigateActionAsync();
            if (preventNavigation)
            {
                context.PreventNavigation();
            }
            else
            {
                await OnLocationChanging(context);
            }
        }

        protected virtual async ValueTask OnLocationChanging(LocationChangingContext context)
        {
            if (stackBottomUri == _navigationManager.ToAbsoluteUri(context.TargetLocation).ToString())
            {
                await HandleNavigateToStackBottomUri();
                context.PreventNavigation();
            }
        }

        protected virtual Task HandleNavigateToStackBottomUri()
        {
            return Task.CompletedTask;
        }

        protected async ValueTask<bool> HandleNavigateActionAsync()
        {
            var historyAction = historyActions.LastOrDefault(it => it.Uri == _navigationManager.Uri);
            if (historyAction is not null)
            {
                historyAction.Action?.Invoke();
                if (historyAction.Func is not null)
                {
                    await historyAction.Func.Invoke();
                }

                return historyAction.PreventNavigation;
            }
            else
            {
                return false;
            }
        }

        private void AddHistoryAction(Action? action = null, Func<Task>? func = null, bool preventNavigation = true)
        {
            var uri = _navigationManager.Uri;
            historyActions.Add(new()
            {
                Uri = uri,
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
    }
}
