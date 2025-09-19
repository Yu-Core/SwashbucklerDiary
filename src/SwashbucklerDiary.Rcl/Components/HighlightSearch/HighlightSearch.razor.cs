using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Extensions;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class HighlightSearch : OpenCloseComponentBase
    {
        private bool _visible;

        private int count;

        private int searchIndex;

        private string? _previousSelector;

        private string? _previousValue;

        private HighlightSearchJSObjectReferenceProxy? _betterSearchProxy;

        private HighlightSearchJSModule? jSModule;

        private DotNetObjectReference<object>? _dotNetObjectReference;

        [Parameter]
        public override bool Visible
        {
            get => _visible;
            set => SetVisible(value);
        }

        [Parameter]
        public string? Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public string? Selector { get; set; } = default!;

        [Parameter]
        public bool Autofocus { get; set; } = true;

        [JSInvokable]
        public void UpdateSearchIndex(int value)
        {
            searchIndex = value;
        }

        [JSInvokable]
        public void UpdateCount(int value)
        {
            count = value;
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (_previousSelector != Selector)
            {
                _previousSelector = Selector;

                await InitBetterSearchAsync();
            }

            if (_previousValue != Value)
            {
                _previousValue = Value;

                await Search(Value);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (!IsDisposed && firstRender)
            {
                jSModule = new(JS);
                _dotNetObjectReference = DotNetObjectReference.Create<object>(this);

                await InitBetterSearchAsync();
                if (!string.IsNullOrWhiteSpace(Value))
                {
                    await Search(Value);
                    StateHasChanged();
                }
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            if (Visible)
            {
                NavigateController.RemoveHistoryAction(CloseSearch);
            }

            _dotNetObjectReference?.Dispose();
            await jSModule.TryDisposeAsync();
        }

        private async Task InitBetterSearchAsync()
        {
            if (IsDisposed || jSModule is null || _dotNetObjectReference is null) return;

            if (_betterSearchProxy is not null)
            {
                await _betterSearchProxy.DisposeAsync();
            }

            _betterSearchProxy = await jSModule.Init(Selector, _dotNetObjectReference);
        }

        private async Task Search(string? text)
        {
            if (_betterSearchProxy is null) return;
            if (string.IsNullOrWhiteSpace(text))
            {
                await _betterSearchProxy.Clear();
            }
            else
            {
                await _betterSearchProxy.Search(text);
            }
        }

        private async Task Up()
        {
            if (_betterSearchProxy is null) return;
            await _betterSearchProxy.Up();
        }

        private async Task Down()
        {
            if (_betterSearchProxy is null) return;
            await _betterSearchProxy.Down();
        }

        private async Task Clear()
        {
            if (_betterSearchProxy is null) return;
            await _betterSearchProxy.Clear();
        }

        private async void SetVisible(bool value)
        {
            if (_visible == value)
            {
                return;
            }

            _visible = value;

            if (value)
            {
                NavigateController.AddHistoryAction(CloseSearch);
            }
            else
            {
                NavigateController.RemoveHistoryAction(CloseSearch);
                Value = string.Empty;
                if (ValueChanged.HasDelegate)
                {
                    await ValueChanged.InvokeAsync(Value);
                }

                await Clear();
            }
        }

        private void CloseSearch()
        {
            InvokeAsync(async () =>
            {
                await InternalVisibleChanged(false);
            });
        }

        private async Task HandleValueChanged(string value)
        {
            Value = _previousValue = value;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(value);
            }
        }
    }
}