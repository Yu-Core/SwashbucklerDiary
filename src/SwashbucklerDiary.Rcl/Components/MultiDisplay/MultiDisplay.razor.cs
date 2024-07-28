using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MultiDisplay : IDisposable
    {
        private bool IsDesktop => MdAndUp ? MasaBlazorHelper.Breakpoint.MdAndUp : MasaBlazorHelper.Breakpoint.SmAndUp;

        [Inject]
        public MasaBlazorHelper MasaBlazorHelper { get; set; } = default!;

        [Parameter]
        public RenderFragment? MobileContent { get; set; }

        [Parameter]
        public RenderFragment? DesktopContent { get; set; }

        [Parameter]
        public bool MdAndUp { get; set; }

        [Parameter]
        public EventCallback<bool> OnUpdate { get; set; }


        public void Dispose()
        {
            MasaBlazorHelper.BreakpointChanged -= HandleBreakpointChange;
            GC.SuppressFinalize(this);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            MasaBlazorHelper.BreakpointChanged += HandleBreakpointChange;
        }

        private async void HandleBreakpointChange(object? sender, MyBreakpointChangedEventArgs e)
        {
            var changed = MdAndUp ? e.MdAndUpChanged : e.SmAndUpChanged;
            if (!changed)
            {
                return;
            }

            if (OnUpdate.HasDelegate)
            {
                await OnUpdate.InvokeAsync(IsDesktop);
            }

            await InvokeAsync(StateHasChanged);
        }
    }
}
