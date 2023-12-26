using Masa.Blazor;
using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
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

        [Parameter]
        public EventCallback<bool> OnUpdate { get; set; }

        private bool Show;

        public void Dispose()
        {
            MasaBlazor.BreakpointChanged -= InvokeStateHasChanged;
            GC.SuppressFinalize(this);
        }

        protected override Task OnInitializedAsync()
        {
            Show = Tablet ? MasaBlazor.Breakpoint.MdAndUp : MasaBlazor.Breakpoint.SmAndUp;
            MasaBlazor.BreakpointChanged += InvokeStateHasChanged;
            return base.OnInitializedAsync();
        }

        private async void InvokeStateHasChanged(object? sender, BreakpointChangedEventArgs e)
        {
            var show = Tablet ? MasaBlazor.Breakpoint.MdAndUp : MasaBlazor.Breakpoint.SmAndUp;
            bool update = Show != show;
            if(update)
            {
                if (OnUpdate.HasDelegate)
                {
                    await OnUpdate.InvokeAsync(Show);
                }

                Show = show;
                StateHasChanged();
            }
        }
    }
}
