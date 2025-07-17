using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class HighlightSearchTextField : OpenCloseComponentBase
    {
        private bool _visible;

        private string? _previousSelector;

        private string? _previousValue;

        private BetterSearchJSObjectReferenceProxy? _betterSearchProxy;

        [Inject]
        private BetterSearchJSModule BetterSearchJSModule { get; set; } = default!;

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

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (_previousSelector != Selector)
            {
                _previousSelector = Selector;

                if (_betterSearchProxy is null)
                {
                    return;
                }

                await InitBetterSearchAsync();
            }

            if (_previousValue != Value)
            {
                _previousValue = Value;

                if (_betterSearchProxy is null)
                {
                    return;
                }

                await HighlightSearch(Value);
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await InitBetterSearchAsync();
                if (!string.IsNullOrWhiteSpace(Value))
                {
                    await HighlightSearch(Value);
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
        }

        private int Count => _betterSearchProxy?.Count ?? 0;

        private int CurrentNumber => Count == 0 ? 0 : _betterSearchProxy!.SearchIndex + 1;

        private async Task InitBetterSearchAsync()
        {
            if (_betterSearchProxy is not null)
            {
                await _betterSearchProxy.Clear();
            }

            _betterSearchProxy = await BetterSearchJSModule.Init(Selector);
        }

        private async Task HighlightSearch(string? text)
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