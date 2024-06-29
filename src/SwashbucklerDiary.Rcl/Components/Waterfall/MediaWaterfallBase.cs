using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public abstract class MediaWaterfallBase : MediaResourceListComponentBase, IDisposable
    {
        protected ElementReference elementReference = default!;

        [Inject]
        protected IJSRuntime JS { get; set; } = default!;

        [Inject]
        protected MasaBlazor MasaBlazor { get; set; } = default!;

        [Inject]
        protected PreviewMediaElementJSModule PreviewMediaElementJSModule { get; set; } = default!;

        public void Dispose()
        {
            MasaBlazor.BreakpointChanged -= InvokeStateHasChanged;
            GC.SuppressFinalize(this);
        }

        protected int Gap => MasaBlazor.Breakpoint.Xs ? 16 : 24;

        protected int Cols => MasaBlazor.Breakpoint.Xs ? 2 : 3;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            MasaBlazor.BreakpointChanged += InvokeStateHasChanged;
        }

        protected async void InvokeStateHasChanged(object? sender, BreakpointChangedEventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
