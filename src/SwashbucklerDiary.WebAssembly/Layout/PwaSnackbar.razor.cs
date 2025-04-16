using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.WebAssembly.Layout
{
    public partial class PwaSnackbar : IDisposable
    {
        [Inject]
        private BreakpointService BreakpointService { get; set; } = default!;

        [CascadingParameter(Name = "Culture")]
        public string? Culture { get; set; }

        [Parameter]
        public Func<Task>? OnRefresh { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            BreakpointService.BreakpointChanged += HandleBreakpointChange;
        }

        private bool Top => BreakpointService.Breakpoint.Xs;

        private bool Rigtht => BreakpointService.Breakpoint.SmAndUp;

        private bool Bottom => BreakpointService.Breakpoint.SmAndUp;

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
            BreakpointService.BreakpointChanged -= HandleBreakpointChange;
            GC.SuppressFinalize(this);
        }
    }
}