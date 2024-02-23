using Masa.Blazor;
using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MultiDisplay : IDisposable
    {
        private bool _isDesktop;

        [Inject]
        public MasaBlazor MasaBlazor { get; set; } = default!;

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
            MasaBlazor.BreakpointChanged -= InvokeStateHasChanged;
            GC.SuppressFinalize(this);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _isDesktop = MdAndUp ? MasaBlazor.Breakpoint.MdAndUp : MasaBlazor.Breakpoint.SmAndUp;
            MasaBlazor.BreakpointChanged += InvokeStateHasChanged;
        }

        private async void InvokeStateHasChanged(object? sender, BreakpointChangedEventArgs e)
        {
            var isDesktop = MdAndUp ? MasaBlazor.Breakpoint.MdAndUp : MasaBlazor.Breakpoint.SmAndUp;
            bool update = _isDesktop != isDesktop;
            if (update)
            {
                if (OnUpdate.HasDelegate)
                {
                    await OnUpdate.InvokeAsync(_isDesktop);
                }

                _isDesktop = isDesktop;
                await InvokeAsync(StateHasChanged);
            }
        }
    }
}
