using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class RightBottomFloatButton : IDisposable
    {
        [Inject]
        private MasaBlazorHelper MasaBlazorHelper { get; set; } = default!;

        [Parameter]
        public string? Icon { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        public void Dispose()
        {
            MasaBlazorHelper.BreakpointChanged -= InvokeStateHasChanged;
            GC.SuppressFinalize(this);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            MasaBlazorHelper.BreakpointChanged += InvokeStateHasChanged;
        }

        private bool Desktop => MasaBlazorHelper.Breakpoint.MdAndUp;

        private void InvokeStateHasChanged(object? sender, MyBreakpointChangedEventArgs e)
        {
            if (!e.MdAndUpChanged)
            {
                return;
            }

            InvokeAsync(StateHasChanged);
        }
    }
}
