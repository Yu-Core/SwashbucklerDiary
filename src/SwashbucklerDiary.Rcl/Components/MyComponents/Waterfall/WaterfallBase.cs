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

        protected List<ResourceModel> LoadedItems = [];

        [Inject]
        protected IJSRuntime JS { get; set; } = default!;

        [Inject]
        protected MasaBlazor MasaBlazor { get; set; } = default!;

        [CascadingParameter(Name = "ScrollElementId")]
        public string? ScrollElementId { get; set; }

        [Parameter]
        public List<ResourceModel> Value { get; set; } = [];

        public async ValueTask DisposeAsync()
        {
            await OnDisposeAsync();
            GC.SuppressFinalize(this);
        }

        protected int Gap => MasaBlazor.Breakpoint.Xs ? 16 : 24;

        protected int Cols => MasaBlazor.Breakpoint.Xs ? 2 : 3;

        protected bool ShowLoadMore => LoadedItems.Count != 0 && LoadedItems.Count < Value.Count;

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
                int loadedCount = LoadedItems.Count;
                LoadedItems = [];
                LoadedItems = MockRequest(loadedCount);
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

            LoadedItems.AddRange(append);
        }

        protected virtual List<ResourceModel> MockRequest(int requestCount = 0)
        {
            if (requestCount < loadCount)
            {
                requestCount = loadCount;
            }

            return Value.Skip(LoadedItems.Count).Take(requestCount).ToList();
        }

        protected async void InvokeStateHasChanged(object? sender, BreakpointChangedEventArgs e)
        {
            await InvokeAsync(StateHasChanged);
        }
    }
}
