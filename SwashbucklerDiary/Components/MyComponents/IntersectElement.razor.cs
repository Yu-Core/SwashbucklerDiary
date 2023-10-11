using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Components
{
    public partial class IntersectElement : IAsyncDisposable
    {
        private ElementReference Ref;

        [Inject]
        private IntersectJSModule IntersectJSModule { get; set; } = default!;

        [Parameter]
        public string? Class { get;set; }

        [Parameter]
        public string? Style { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public EventCallback<bool> IntersectChanged { get; set; }


        public async ValueTask DisposeAsync()
        {
            await IntersectJSModule.UnobserveAsync(Ref);
            GC.SuppressFinalize(this);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var handle = DotNetObjectReference.Create(new IntersectInvoker(OnIntersectAsync));
                await IntersectJSModule.ObserverAsync(Ref, handle);
            }
        }

        private async Task OnIntersectAsync(IntersectEventArgs args)
        {
            if (IntersectChanged.HasDelegate)
            {
                await IntersectChanged.InvokeAsync(args.IsIntersecting);
            }

            await InvokeAsync(StateHasChanged);
        }
    }
}
