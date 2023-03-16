using BlazorComponent;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Components
{
    public partial class SwiperTabItems : IAsyncDisposable
    {
        private IJSObjectReference? module;
        private StringNumber _value = 0;
        private bool Show;
        private readonly string Id = "swiper" + Guid.NewGuid().ToString();

        [Inject]
        private IJSRuntime? JS { get; set; }

        [Parameter]
        public StringNumber Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    if (Show)
                    {
                        UpdateSwiper(value);
                    }
                }
            }
        }
        [Parameter]
        public EventCallback<StringNumber> ValueChanged { get; set; }
        [Parameter]
        public RenderFragment? ChildContent { get; set; }

        [JSInvokable]
        public async Task UpdateValue(int value)
        {
            _value = value;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(value);
            }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Show = Value == 0;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                module = await JS!.InvokeAsync<IJSObjectReference>("import", "./js/init-swiper.js");
                var dotNetCallbackRef = DotNetObjectReference.Create(this);
                await module.InvokeVoidAsync("swiperInit", new object[4] { dotNetCallbackRef, "UpdateValue", Id, Value.ToInt32() });
                Show = true;
                StateHasChanged();
            }
        }

        private async void UpdateSwiper(StringNumber value)
        {
            await JS!.InvokeVoidAsync($"{Id}.slideTo", new object[1] { value.ToInt32() });
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (module is not null)
            {
                await JS!.InvokeVoidAsync($"{Id}.destroy", new object[2] { true, true });
                await module.DisposeAsync();
            }

            GC.SuppressFinalize(this);
        }
    }
}
