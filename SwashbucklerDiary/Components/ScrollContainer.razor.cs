using Masa.Blazor;
using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Components
{
    public partial class ScrollContainer : IDisposable
    {
        [Inject]
        public MasaBlazor MasaBlazor { get; set; } = default!;

        [Parameter]
        public RenderFragment? ChildContent { get; set; }
        [Parameter]
        public string? Id { get; set; }
        [Parameter]
        public string? Class { get; set; }
        [Parameter]
        public string? ContentClass { get; set; }
        [Parameter]
        public bool Bottom { get; set; }
        [Parameter]
        public double OccupyHeight { get; set; }
        [Parameter]
        public EventCallback OnContextmenu { get; set; }

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

        private double Height
        {
            get
            {
                var height = MasaBlazor.Breakpoint.Height;
                var top = MasaBlazor.Application.Top;
                var bottom = Bottom ? MasaBlazor.Application.Bottom : 0;
                return height - top - bottom - OccupyHeight;
            }
        }
            

        private async Task InvokeStateHasChangedAsync()
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
