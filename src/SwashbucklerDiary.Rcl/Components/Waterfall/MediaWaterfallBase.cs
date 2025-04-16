using Masa.Blazor;
using Masa.Blazor.Core;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Components
{
    public abstract class MediaWaterfallBase : MediaResourceListComponentBase
    {
        protected bool contentLoading;

        protected string? thisPagePath;

        protected ElementReference elementReference = default!;

        [Inject]
        protected BreakpointService BreakpointService { get; set; } = default!;

        [Inject]
        protected PreviewMediaElementJSModule PreviewMediaElementJSModule { get; set; } = default!;

        [Inject]
        protected WaterfallJSModule WaterfallJSModule { get; set; } = default!;

        protected string WrapStyle => StyleBuilder.Create()
            .AddIf("opacity", "0", contentLoading)
            .Build();

        protected int Gap => BreakpointService.Breakpoint.Xs ? 16 : 24;

        protected int Cols => BreakpointService.Breakpoint.Xs ? 2 : 3;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            thisPagePath = NavigationManager.GetAbsolutePath();
            BreakpointService.BreakpointChanged += HandleBreakpointChange;
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

            BreakpointService.BreakpointChanged -= HandleBreakpointChange;
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
