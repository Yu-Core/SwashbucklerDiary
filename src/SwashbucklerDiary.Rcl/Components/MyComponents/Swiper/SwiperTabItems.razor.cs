using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class SwiperTabItems : IAsyncDisposable
    {
        private StringNumber previousvalue = 0;

        private int _registeredTabItemsIndex;

        [Inject]
        private SwiperJsModule SwiperJsModule { get; set; } = default!;

        [Parameter]
        public StringNumber Value { get; set; } = 0;

        [Parameter]
        public EventCallback<StringNumber> ValueChanged { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        public ElementReference Ref { get; set; }

        public SwiperTabItem? ActiveItem
            => ChildTabItems.Count == 0 ? null : ChildTabItems[Value.ToInt32()];

        public List<SwiperTabItem> ChildTabItems { get; } = [];

        [JSInvokable]
        public async Task UpdateValue(int value)
        {
            Value = value;
            previousvalue = value;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(value);
            }
        }

        public void RegisterTabItem(SwiperTabItem tabItem)
        {
            tabItem.Value ??= _registeredTabItemsIndex++;

            if (ChildTabItems.Any(item => item.Value != null && item.Value.Equals(tabItem.Value))) return;

            ChildTabItems.Add(tabItem);
        }

        public void UnregisterTabItem(SwiperTabItem tabItem)
        {
            ChildTabItems.Remove(tabItem);
        }

        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync();

            if (previousvalue != Value)
            {
                previousvalue = Value;
                await SwiperJsModule.SlideToAsync(Ref, Value.ToInt32());
            }
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                var dotNetObjectReference = DotNetObjectReference.Create<object>(this);
                await SwiperJsModule.Init(dotNetObjectReference, Ref, Value.ToInt32());
            }
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            await SwiperJsModule.DisposeAsync(Ref);
            GC.SuppressFinalize(this);
        }
    }
}
