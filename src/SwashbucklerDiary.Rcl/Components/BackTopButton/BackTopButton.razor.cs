using Masa.Blazor.Core;
using Masa.Blazor.Extensions;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Extensions;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class BackTopButton : MyComponentBase, IAsyncDisposable
    {
        private bool _show;

        private string? _previousSelector;

        private BackTopButtonJSObjectReference? backTopButtonJSObjectReference;

        private DotNetObjectReference<object>? _dotNetObjectReference;

        private MyFloatButton? myFloatButton;

        private BackTopButtonJSModule? jSModule;

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
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (_previousSelector != Selector)
            {
                await RemoveScrollListener();
                await AddScrollListener();

                _previousSelector = Selector;
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!IsDisposed && firstRender)
            {
                jSModule = new(JS);
                _dotNetObjectReference = DotNetObjectReference.Create<object>(this);
                await AddScrollListener();
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            _dotNetObjectReference?.Dispose();
            await RemoveScrollListener();
            await jSModule.TryDisposeAsync();
        }

        private string InternalClass => new CssBuilder()
            .Add("transition-swing")
            .Add(Class)
            .ToString();

        private async Task BackTop()
        {
            if (string.IsNullOrEmpty(Selector)) return;
            await JS.ScrollTo(Selector, 0);
        }

        private async Task AddScrollListener()
        {
            if (IsDisposed
                || jSModule is null
                || _dotNetObjectReference is null
                || string.IsNullOrEmpty(Selector)
                || myFloatButton?.Ref is null) return;

            backTopButtonJSObjectReference = await jSModule.Init(Selector, myFloatButton.Ref, _dotNetObjectReference);
        }

        private async Task RemoveScrollListener()
        {
            if (IsDisposed
                || backTopButtonJSObjectReference is null
                || string.IsNullOrEmpty(_previousSelector)) return;

            _show = false;
            await backTopButtonJSObjectReference.RemoveScrollListener();
            await backTopButtonJSObjectReference.DisposeAsync();
        }
    }
}