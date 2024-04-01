using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public class WaterfallBase : MyComponentBase, IAsyncDisposable
    {
        protected readonly int loadCount = 20;

        protected bool firstLoad = true;

        protected IJSObjectReference module = default!;

        protected ElementReference elementReference = default!;

        protected List<ResourceModel> previousValue = [];

        protected List<string?> srcs = [];

        [Inject]
        protected IJSRuntime JS { get; set; } = default!;

        [Inject]
        protected MasaBlazor MasaBlazor { get; set; } = default!;

        [CascadingParameter(Name = "ScrollElement")]
        public ElementReference ScrollElement { get; set; }

        [Parameter]
        public List<ResourceModel> Value { get; set; } = [];

        public async ValueTask DisposeAsync()
        {
            await OnDisposeAsync();
            GC.SuppressFinalize(this);
        }

        protected int Gap => MasaBlazor.Breakpoint.Xs ? 16 : 24;

        protected int Cols => MasaBlazor.Breakpoint.Xs ? 2 : 3;

        protected bool ShowLoadMore => srcs.Count != 0 && srcs.Count < Value.Count;

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
                int previousValueCount = previousValue.Count;
                previousValue = Value;
                srcs = [];
                srcs = MockRequest(previousValueCount);
            }
        }

        protected virtual async Task OnDisposeAsync()
        {
            MasaBlazor.BreakpointChanged -= InvokeStateHasChanged;
            if (module is not null)
            {
                await module.DisposeAsync();
            }
        }

        protected void OnLoad(InfiniteScrollLoadEventArgs args)
        {
            if (firstLoad)
            {
                firstLoad = false;
                return;
            }

            var append = MockRequest();

            srcs.AddRange(append);
        }

        protected virtual List<string?> MockRequest(int requestCount = 0)
        {
            if (requestCount < loadCount)
            {
                requestCount = loadCount;
            }

            return Value.Skip(srcs.Count).Take(requestCount).Select(it => it.ResourceUri).ToList();
        }

        protected async void InvokeStateHasChanged(object? sender, BreakpointChangedEventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
