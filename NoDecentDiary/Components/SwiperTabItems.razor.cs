using BlazorComponent;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Components
{
    public partial class SwiperTabItems : IAsyncDisposable
    {
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
                        RefreshData.InvokeAsync(value);
                        UpdateSwiper(value);
                    }
                }
            }
        }
        [Parameter]
        public EventCallback<StringNumber> ValueChanged { get; set; }
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
        [Parameter]
        public EventCallback<StringNumber> RefreshData { get; set; }

        private IJSObjectReference? module;
        private StringNumber _value = 0;
        private bool Show;
        private readonly string Id = "swiper" + Guid.NewGuid().ToString();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                module = await JS!.InvokeAsync<IJSObjectReference>("import", "./js/init-swiper.js");
                var dotNetCallbackRef = DotNetObjectReference.Create(this);
                await module.InvokeVoidAsync("swiperInit", new object[4] { dotNetCallbackRef, "UpdateValue", Id, Value.ToInt32() });
                await Task.Delay(100);
                Show = true;
                StateHasChanged();
            }
        }
        private async void UpdateSwiper(StringNumber value)
        {
            await JS!.InvokeVoidAsync($"{Id}.slideTo", new object[1] { value.ToInt32() });
        }
        [JSInvokable]
        public async Task UpdateValue(int value)
        {
            _value = value;
            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(value);
            }
            await RefreshData.InvokeAsync(value);
        }

        async ValueTask IAsyncDisposable.DisposeAsync()
        {
            await JS!.InvokeVoidAsync($"{Id}.destroy", new object[2] { true, true });
            if (module is not null)
            {
                await module.DisposeAsync();
            }

            GC.SuppressFinalize(this);
        }
    }
}
