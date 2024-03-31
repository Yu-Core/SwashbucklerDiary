using BlazorComponent;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Extensions;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class SwiperTabItems : IAsyncDisposable
    {
        private ElementReference ElementRef;

        private IJSObjectReference module = default!;

        private StringNumber _value = 0;

        private readonly List<SwiperTabItem> ChildTabItems = [];

        private int _registeredTabItemsIndex;

        [Inject]
        private IJSRuntime JS { get; set; } = default!;

        [Parameter]
        public StringNumber Value
        {
            get => _value;
            set => SetValue(value);
        }

        [Parameter]
        public EventCallback<StringNumber> ValueChanged { get; set; }

        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        public SwiperTabItem? ActiveItem
        {
            get
            {
                if (ChildTabItems.Count == 0)
                {
                    return null;
                }

                return ChildTabItems[_value.ToInt32()];
            }
        }

        [JSInvokable]
        public async Task UpdateValue(int value)
        {
            _value = value;
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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                var dotNetObjectReference = DotNetObjectReference.Create<object>(this);
                module = await JS.ImportRclJsModule("js/swiper-helper.js");
                await module.InvokeVoidAsync("initSwiper", [dotNetObjectReference, "UpdateValue", ElementRef, Value.ToInt32()]);
            }
        }

        private void SetValue(StringNumber value)
        {
            if (_value != value)
            {
                _value = value;
                _ = UpdateSwiper(value);
            }
        }

        private async Task UpdateSwiper(StringNumber value)
        {
            if (module is null)
            {
                return;
            }

            await module.InvokeVoidAsync("slideTo", [ElementRef, value.ToInt32()]);
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (module is not null)
            {
                await module.InvokeVoidAsync("dispose", ElementRef);
                await module.DisposeAsync();
            }

            GC.SuppressFinalize(this);
        }
    }
}
