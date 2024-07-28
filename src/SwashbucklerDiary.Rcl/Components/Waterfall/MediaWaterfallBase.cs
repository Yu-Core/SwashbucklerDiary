using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Components
{
    public abstract class MediaWaterfallBase : MediaResourceListComponentBase, IDisposable
    {
        protected ElementReference elementReference = default!;

        [Inject]
        protected IJSRuntime JS { get; set; } = default!;

        [Inject]
        protected MasaBlazorHelper MasaBlazorHelper { get; set; } = default!;

        [Inject]
        protected PreviewMediaElementJSModule PreviewMediaElementJSModule { get; set; } = default!;

        public void Dispose()
        {
            MasaBlazorHelper.BreakpointChanged -= HandleBreakpointChange;
            GC.SuppressFinalize(this);
        }

        protected int Gap => MasaBlazorHelper.Breakpoint.Xs ? 16 : 24;

        protected int Cols => MasaBlazorHelper.Breakpoint.Xs ? 2 : 3;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            MasaBlazorHelper.BreakpointChanged += HandleBreakpointChange;
        }

        protected async void HandleBreakpointChange(object? sender, MyBreakpointChangedEventArgs e)
        {
            if (!e.XsChanged)
            {
                return;
            }

            await InvokeAsync(StateHasChanged);
        }
    }
}
