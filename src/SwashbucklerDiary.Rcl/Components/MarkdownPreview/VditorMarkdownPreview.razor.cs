using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class VditorMarkdownPreview
    {
        private string? previousValue;

        private bool previousOutline;

        private bool afterFirstRender;

        private Lazy<DotNetObjectReference<object>> dotNetObjectReference = default!;

        private ElementReference outlineElementRef;

        [Inject]
        private NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        private VditorMarkdownPreviewJSModule VditorMarkdownPreviewJSModule { get; set; } = default!;

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public string? Class { get; set; }

        [Parameter]
        public string? Style { get; set; }

        [Parameter]
        public string? OutlineClass { get; set; }

        [Parameter]
        public string? OutlineStyle { get; set; }

        [Parameter]
        public bool Outline { get; set; }

        [Parameter]
        public bool RightOutline { get; set; }

        [Parameter]
        public bool Simple { get; set; }

        [Parameter]
        public bool Patch { get; set; } = true;

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

        [JSInvokable]
        public void NavigateToReplace(string url)
        {
            NavigationManager.NavigateTo(url, replace: true);
        }

        public async Task RenderLazyLoadingImage()
        {
            await VditorMarkdownPreviewJSModule.RenderLazyLoadingImage(Ref);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            dotNetObjectReference = new(() => DotNetObjectReference.Create<object>(this));
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (previousValue != Value)
            {
                previousValue = Value;
                await RenderMarkdown();
            }

            if (previousOutline != Outline)
            {
                previousOutline = Outline;
                await RenderOutline();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                afterFirstRender = true;
                await RenderMarkdown();
            }
        }

        private async Task RenderMarkdown()
        {
            if (!afterFirstRender)
            {
                return;
            }

            if (Simple)
            {
                await VditorMarkdownPreviewJSModule.Md2HTMLPreview(dotNetObjectReference.Value, Ref, Value, Options, Patch);
            }
            else
            {
                await VditorMarkdownPreviewJSModule.Preview(dotNetObjectReference.Value, Ref, Value, Options, Outline ? outlineElementRef : null, Patch);
            }
        }

        private async Task RenderOutline()
        {
            if (!afterFirstRender || !Outline || Simple)
            {
                return;
            }

            await Task.Delay(200);
            await VditorMarkdownPreviewJSModule.RenderOutline(Ref, outlineElementRef);
        }
    }
}
