using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class IntersectElement : IAsyncDisposable
    {
        private ElementReference elementReference;

        private DotNetObjectReference<IntersectInvoker>? _dotNetObjectReference;

        [Inject]
        private IntersectJSModule IntersectJSModule { get; set; } = default!;

        [Parameter]
        public string? Class { get; set; }

        [Parameter]
        public string? Style { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [Parameter]
        public EventCallback<bool> IntersectChanged { get; set; }

        public async ValueTask DisposeAsync()
        {
            await IntersectJSModule.UnobserveAsync(elementReference);
            _dotNetObjectReference?.Dispose();
            GC.SuppressFinalize(this);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                _dotNetObjectReference ??= DotNetObjectReference.Create(new IntersectInvoker(OnIntersectAsync));
                await IntersectJSModule.ObserverAsync(elementReference, _dotNetObjectReference);
            }
        }

        private async Task OnIntersectAsync(IntersectEventArgs args)
        {
            if (IntersectChanged.HasDelegate)
            {
                await IntersectChanged.InvokeAsync(args.IsIntersecting);
            }
        }
    }
}
