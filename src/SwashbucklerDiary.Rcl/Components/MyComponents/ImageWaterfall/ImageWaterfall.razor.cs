using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class ImageWaterfall : IAsyncDisposable
    {
        private readonly int loadCount = 20;

        private bool showPreviewImage;

        private string? previewImageSrc;

        private bool firstLoad = true;

        private IJSObjectReference module = default!;

        private ElementReference elementReference = default!;

        private List<ResourceModel> previousValue = [];

        private List<string?> imageSrcs = [];

        [Inject]
        private IJSRuntime JS { get; set; } = default!;

        [Inject]
        public MasaBlazor MasaBlazor { get; set; } = default!;

        [CascadingParameter(Name = "ScrollElement")]
        public ElementReference ScrollElement { get; set; }

        [Parameter]
        public List<ResourceModel> Value { get; set; } = [];

        private bool ShowLoadMore => imageSrcs.Count != 0 && imageSrcs.Count < Value.Count;

        [JSInvokable]
        public async Task PreviewImage(string src)
        {
            previewImageSrc = src;
            showPreviewImage = true;
            await InvokeAsync(StateHasChanged);
        }

        public async ValueTask DisposeAsync()
        {
            if (module is not null)
            {
                await module.DisposeAsync();
            }

            MasaBlazor.BreakpointChanged -= InvokeStateHasChanged;
            GC.SuppressFinalize(this);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            MasaBlazor.BreakpointChanged += InvokeStateHasChanged;
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            if (previousValue != Value)
            {
                previousValue = Value;
                imageSrcs = [];
                imageSrcs = MockRequest();
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                module = await JS.ImportRclJsModule("js/previewImage.js");
                var dotNetCallbackRef = DotNetObjectReference.Create(this);

                //图片预览
                await module.InvokeVoidAsync("previewImage", [dotNetCallbackRef, "PreviewImage", elementReference]);
            }
        }

        private int Gap => MasaBlazor.Breakpoint.Xs ? 16 : 24;

        private int Cols => MasaBlazor.Breakpoint.Xs ? 2 : 3;

        private void OnLoad(InfiniteScrollLoadEventArgs args)
        {
            if (firstLoad)
            {
                firstLoad = false;
                return;
            }

            var append = MockRequest();

            imageSrcs.AddRange(append);
        }

        private List<string?> MockRequest()
        {
            return Value.Skip(imageSrcs.Count).Take(loadCount).Select(it => it.ResourceUri).ToList();
        }

        private async void InvokeStateHasChanged(object? sender, BreakpointChangedEventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
