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

        private bool Show;

        private readonly string Id = $"swiper-{Guid.NewGuid()}";

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
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                module = await JS.ImportRclJsModule("js/swiper-helper.js");
                var dotNetCallbackRef = DotNetObjectReference.Create(this);
                await module.InvokeVoidAsync("initSwiper", [dotNetCallbackRef, "UpdateValue", ElementRef, $"#{Id}", Value.ToInt32()]);
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
            await module.InvokeVoidAsync("slideTo", [ElementRef, value.ToInt32()]);
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
