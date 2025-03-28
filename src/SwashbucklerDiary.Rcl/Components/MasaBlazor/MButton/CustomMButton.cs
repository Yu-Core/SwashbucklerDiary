using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public class CustomMButton : MButton
    {
        [Parameter]
        public bool OnMousedownPreventDefault { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender && OnMousedownPreventDefault)
            {
                await Js.InvokeVoidAsync("preventDefaultOnmousedown", Ref);
            }
        }
    }
}
