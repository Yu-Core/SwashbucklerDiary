using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Extensions;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class VditorMarkdownPreview
    {
        private string? previousValue;

        private DotNetObjectReference<object>? _dotNetObjectReference;

        private VditorMarkdownPreviewJSModule? jSModule;

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public string? Class { get; set; }

        [Parameter]
        public string? Style { get; set; }

        [Parameter]
        public bool Simple { get; set; }

        [Parameter]
        public bool Optimize { get; set; } = true;

        [Parameter]
        public Dictionary<string, object>? Options { get; set; }

        [Parameter]
        public EventCallback OnAfter { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnDblClick { get; set; }

        public ElementReference Ref { get; set; }

        [JSInvokable]
        public async Task After()
        {
            if (OnAfter.HasDelegate)
            {
                await OnAfter.InvokeAsync();
            }
        }

        [JSInvokable]
        public void NavigateToReplace(string url)
        {
            NavigationManager.NavigateTo(url, replace: true);
        }

        public async Task RenderLazyLoadingImage()
        {
            if (jSModule is not null)
            {
                await jSModule.RenderLazyLoadingImage(Ref);
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (previousValue != Value)
            {
                previousValue = Value;
                await RenderMarkdown();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!IsDisposed && firstRender)
            {
                _dotNetObjectReference = DotNetObjectReference.Create<object>(this);
                jSModule = new(JS);
                await RenderMarkdown();
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            _dotNetObjectReference?.Dispose();
            await jSModule.TryDisposeAsync();
        }

        private async Task RenderMarkdown()
        {
            if (IsDisposed || jSModule is null || _dotNetObjectReference is null)
            {
                return;
            }

            if (Simple)
            {
                await jSModule.Md2HTMLPreview(_dotNetObjectReference, Ref, Value, Options, Optimize);
            }
            else
            {
                await jSModule.Preview(_dotNetObjectReference, Ref, Value, Options, Optimize);
            }
        }
    }
}
