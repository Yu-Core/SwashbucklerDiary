using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.WebAssembly.Layout
{
    public partial class PwaSnackbar : IDisposable
    {
        [Inject]
        private MasaBlazorHelper MasaBlazorHelper { get; set; } = default!;

        [CascadingParameter(Name = "Culture")]
        public string? Culture { get; set; }

        [Parameter]
        public Func<Task>? OnRefresh { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            MasaBlazorHelper.BreakpointChanged += HandleBreakpointChange;
        }

        private bool Top => MasaBlazorHelper.Breakpoint.Xs;

        private bool Rigtht => MasaBlazorHelper.Breakpoint.SmAndUp;

        private bool Bottom => MasaBlazorHelper.Breakpoint.SmAndUp;

        private async Task Ignore()
        {
            await JSRuntime.InvokeVoidAsync("swIgnoreUpdate");
        }

        private void HandleBreakpointChange(object? sender, MyBreakpointChangedEventArgs e)
        {
            if (!e.XsChanged)
            {
                return;
            }

            InvokeAsync(StateHasChanged);
        }

        public void Dispose()
        {
            MasaBlazorHelper.BreakpointChanged -= HandleBreakpointChange;
            GC.SuppressFinalize(this);
        }
    }
}