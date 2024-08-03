using Masa.Blazor.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class BackTopButton : IAsyncDisposable
    {
        private bool isRendered;

        private bool _show;

        private string? _previousSelector;

        private BackTopButtonJSObjectReference? backTopButtonJSObjectReference;

        private DotNetObjectReference<object>? _objRef;

        private MyFloatButton? myFloatButton;

        [Inject]
        IJSRuntime JS { get; set; } = default!;

        [Inject]
        BackTopButtonJSModule Module { get; set; } = default!;

        [Parameter]
        public string? Class { get; set; }

        [Parameter]
        public string? Selector { get; set; }

        [JSInvokable]
        public async Task UpdateShow(bool value)
        {
            _show = value;
            await InvokeAsync(StateHasChanged);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            _objRef = DotNetObjectReference.Create<object>(this);
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (_previousSelector != Selector)
            {
                if (isRendered)
                {
                    await RemoveScrollListener();
                    await AddScrollListener();
                }

                _previousSelector = Selector;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                isRendered = true;
                await AddScrollListener();
            }
        }

        private string InternalClass => $"transition-swing {Class}";

        private async Task BackTop()
        {
            if (string.IsNullOrEmpty(Selector)) return;
            await JS.ScrollTo(Selector, 0);
        }

        private async Task AddScrollListener()
        {
            if (string.IsNullOrEmpty(Selector) || myFloatButton?.Ref is null) return;
            backTopButtonJSObjectReference = await Module.Init(Selector, myFloatButton.Ref, _objRef);
        }

        private async Task RemoveScrollListener()
        {
            if (backTopButtonJSObjectReference is null || string.IsNullOrEmpty(_previousSelector)) return;
            _show = false;
            await backTopButtonJSObjectReference.RemoveScrollListener();
            await backTopButtonJSObjectReference.DisposeAsync();
        }

        public async ValueTask DisposeAsync()
        {
            try
            {
                await RemoveScrollListener();
            }
            catch (JSDisconnectedException)
            {
                // ignore
            }

            _objRef?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}