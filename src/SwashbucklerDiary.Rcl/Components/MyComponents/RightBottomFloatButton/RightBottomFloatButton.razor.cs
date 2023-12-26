using Masa.Blazor;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class RightBottomFloatButton : IDisposable
    {
        [Inject]
        private MasaBlazor MasaBlazor { get; set; } = default!;

        [Parameter]
        public string? Icon { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        public void Dispose()
        {
            MasaBlazor.BreakpointChanged -= InvokeStateHasChanged;
            GC.SuppressFinalize(this);
        }

        protected override Task OnInitializedAsync()
        {
            MasaBlazor.BreakpointChanged += InvokeStateHasChanged;
            return base.OnInitializedAsync();
        }

        private bool Desktop => MasaBlazor.Breakpoint.MdAndUp;

        private void InvokeStateHasChanged(object? sender, BreakpointChangedEventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }
    }
}
