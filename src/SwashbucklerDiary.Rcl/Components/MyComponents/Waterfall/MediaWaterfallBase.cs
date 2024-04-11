using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public abstract class MediaWaterfallBase : MediaResourceListComponentBase, IAsyncDisposable
    {
        protected IJSObjectReference module = default!;

        protected ElementReference elementReference = default!;

        [Inject]
        protected IJSRuntime JS { get; set; } = default!;

        [Inject]
        protected MasaBlazor MasaBlazor { get; set; } = default!;

        public async ValueTask DisposeAsync()
        {
            await OnDisposeAsync();
            GC.SuppressFinalize(this);
        }

        protected int Gap => MasaBlazor.Breakpoint.Xs ? 16 : 24;

        protected int Cols => MasaBlazor.Breakpoint.Xs ? 2 : 3;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            MasaBlazor.BreakpointChanged += InvokeStateHasChanged;
        }

        protected virtual async Task OnDisposeAsync()
        {
            MasaBlazor.BreakpointChanged -= InvokeStateHasChanged;
            if (module is not null)
            {
                await module.DisposeAsync();
            }
        }

        protected async void InvokeStateHasChanged(object? sender, BreakpointChangedEventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
