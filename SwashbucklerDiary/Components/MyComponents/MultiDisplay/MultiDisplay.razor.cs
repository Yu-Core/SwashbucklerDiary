using Masa.Blazor;
using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Components
{
    public partial class MultiDisplay : IDisposable
    {
        [Inject]
        public MasaBlazor MasaBlazor { get; set; } = default!;

        [Parameter]
        public RenderFragment? MobileContent { get; set; }
        [Parameter]
        public RenderFragment? DesktopContent { get; set; }
        [Parameter]
        public bool Tablet { get; set; }

        public void Dispose()
        {
            MasaBlazor.Breakpoint.OnUpdate -= InvokeStateHasChangedAsync;
            GC.SuppressFinalize(this);
        }

        protected override Task OnInitializedAsync()
        {
            MasaBlazor.Breakpoint.OnUpdate += InvokeStateHasChangedAsync;
            return base.OnInitializedAsync();
        }

        private void InvokeStateHasChangedAsync(object? sender, BreakpointChangedEventArgs e)
        {
            StateHasChanged();
        }
    }
}
