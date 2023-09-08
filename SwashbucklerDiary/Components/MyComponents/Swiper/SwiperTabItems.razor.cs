using BlazorComponent;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Components
{
    public partial class SwiperTabItems : IAsyncDisposable
    {
        private ElementReference ElementRef;
        private IJSObjectReference module = default!;
        private StringNumber _value = 0;
        private bool Show;
        private readonly string Id = $"swiper-{ Guid.NewGuid() }";

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
                module = await JS.InvokeAsync<IJSObjectReference>("import", "./js/swiper-helper.js");
                var dotNetCallbackRef = DotNetObjectReference.Create(this);
                await module.InvokeVoidAsync("initSwiper", new object[5] { dotNetCallbackRef, "UpdateValue", ElementRef, $"#{Id}", Value.ToInt32() });
                Show = true;
                await InvokeAsync(StateHasChanged);
            }
        }

        private void SetValue(StringNumber value)
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

        private async void UpdateSwiper(StringNumber value)
        {
            await module.InvokeVoidAsync("slideTo", new object[2] { ElementRef, value.ToInt32() });
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            if (module is not null)
            {
                await module.DisposeAsync();
            }

            GC.SuppressFinalize(this);
        }
    }
}
