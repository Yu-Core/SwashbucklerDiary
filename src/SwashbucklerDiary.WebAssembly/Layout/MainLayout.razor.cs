using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Layout;

namespace SwashbucklerDiary.WebAssembly.Layout
{
    public partial class MainLayout : MainLayoutBase
    {
        [Inject]
        private IJSRuntime JSRuntime { get; set; } = default!;

        private async Task RefreshPage()
        {
            await RefreshToSkipWaiting();
        }

        private async Task RefreshToSkipWaiting()
        {
            await JSRuntime.InvokeVoidAsync("swSkipWaiting");
        }
    }
}
