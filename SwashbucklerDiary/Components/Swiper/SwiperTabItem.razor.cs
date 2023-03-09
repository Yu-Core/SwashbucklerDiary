using Masa.Blazor;
using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Components
{
    public partial class SwiperTabItem : IDisposable
    {
        [Inject]
        public MasaBlazor MasaBlazor { get; set; } = default!;
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

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

        private double Height => MasaBlazor.Breakpoint.Height - MasaBlazor.Application.Top - MasaBlazor.Application.Bottom;

        private async Task InvokeStateHasChangedAsync()
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
