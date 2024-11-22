using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Components
{
    public abstract class MediaWaterfallBase : MediaResourceListComponentBase
    {
        protected bool contentLoading;

        protected string? thisPagePath;

        protected ElementReference elementReference = default!;

        [Inject]
        protected MasaBlazorHelper MasaBlazorHelper { get; set; } = default!;

        [Inject]
        protected PreviewMediaElementJSModule PreviewMediaElementJSModule { get; set; } = default!;

        [Inject]
        protected WaterfallJSModule WaterfallJSModule { get; set; } = default!;

        protected int Gap => MasaBlazorHelper.Breakpoint.Xs ? 16 : 24;

        protected int Cols => MasaBlazorHelper.Breakpoint.Xs ? 2 : 3;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            thisPagePath = NavigationManager.GetAbsolutePath();
            MasaBlazorHelper.BreakpointChanged += HandleBreakpointChange;
            NavigationManager.LocationChanged += NavigationManagerOnLocationChanged;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender && !string.IsNullOrEmpty(ScrollElementId))
            {
                await WaterfallJSModule.RecordScrollInfo($"#{ScrollElementId}");
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            MasaBlazorHelper.BreakpointChanged -= HandleBreakpointChange;
            NavigationManager.LocationChanged -= NavigationManagerOnLocationChanged;
        }

        protected async void HandleBreakpointChange(object? sender, MyBreakpointChangedEventArgs e)
        {
            if (!e.XsChanged)
            {
                return;
            }

            await InvokeAsync(StateHasChanged);
        }

        protected virtual async void NavigationManagerOnLocationChanged(object? sender, LocationChangedEventArgs e)
        {
            var currentPath = NavigationManager.GetAbsolutePath();
            if (thisPagePath != currentPath)
            {
                await StopRecordScrollInfo();
            }
            else
            {
                await RestoreScrollPosition();
            }
        }

        private async Task StopRecordScrollInfo()
        {
            if (string.IsNullOrEmpty(ScrollElementId)) return;
            await WaterfallJSModule.StopRecordScrollInfo($"#{ScrollElementId}");
            contentLoading = true;
            await InvokeAsync(StateHasChanged);
        }

        private async Task RestoreScrollPosition()
        {
            if (string.IsNullOrEmpty(ScrollElementId)) return;
            await Task.Delay(300);
            await WaterfallJSModule.RestoreScrollPosition($"#{ScrollElementId}");
            contentLoading = false;
            await InvokeAsync(StateHasChanged);
        }
    }
}
