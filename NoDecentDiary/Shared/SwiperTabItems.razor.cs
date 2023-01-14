using BlazorComponent;
using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Shared
{
    public partial class SwiperTabItems : IDisposable
    {
        [Inject]
        private IJSRuntime? JSRuntime { get; set; }
        [Parameter]
        public StringNumber Value
        {
            get => _value;
            set
            {
                if (_value != value)
                {
                    _value = value;
                    if(AfterFirstRender)
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

        private StringNumber _value = 0;
        private bool AfterFirstRender;
        private string Id = "swiper" + Guid.NewGuid().ToString();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && ChildContent is not null)
            {
                var dotNetCallbackRef = DotNetObjectReference.Create(this);
                await JSRuntime!.InvokeVoidAsync("swiperInit", new object[4] { dotNetCallbackRef, "UpdateValue", Id, Value.ToInt32() });
                AfterFirstRender = true;
                StateHasChanged();
            }
        }
        private async void UpdateSwiper(StringNumber value)
        {
            await JSRuntime!.InvokeVoidAsync($"{Id}.slideTo", new object[1] { value.ToInt32() });
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
            StateHasChanged();
        }

        public async void Dispose()
        {
            await JSRuntime!.InvokeVoidAsync($"{Id}.destroy", new object[2] { true, true });
            GC.SuppressFinalize(this);
        }
    }
}
