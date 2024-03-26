using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Extensions;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class VditorMarkdownPreview : IAsyncDisposable
    {
        private IJSObjectReference module = default!;

        private DotNetObjectReference<VditorMarkdownPreview> dotNetObjectReference = default!;

        private string? previousValue;

        [Inject]
        public IJSRuntime JS { get; set; } = default!;

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public string? Class { get; set; }

        [Parameter]
        public string? Style { get; set; }

        [Parameter]
        public Dictionary<string, object>? Options { get; set; }

        [Parameter]
        public EventCallback OnAfter { get; set; }

        public ElementReference Ref { get; set; }

        [JSInvokable]
        public async Task After()
        {
            if (OnAfter.HasDelegate)
            {
                await OnAfter.InvokeAsync();
            }
        }

        public async ValueTask DisposeAsync()
        {
            if (module is not null)
            {
                await module.DisposeAsync();
            }

            GC.SuppressFinalize(this);
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (previousValue != Value)
            {
                previousValue = Value;
                await RenderingMarkdown();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                module = await JS.ImportRclJsModule("js/vditor-preview-helper.js");
                await RenderingMarkdown();
            }
        }

        private async Task RenderingMarkdown()
        {
            if (module is null)
            {
                return;
            }

            dotNetObjectReference ??= DotNetObjectReference.Create(this);
            await module.InvokeVoidAsync("previewVditor", [dotNetObjectReference, Ref, Value, Options]);
        }
    }
}
