using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace SwashbucklerDiary.Rcl.Components
{
    public class MButtonExtension : MButton
    {
        [Parameter]
        public bool OnMousedownPreventDefault { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && OnMousedownPreventDefault)
            {
                await Js.InvokeVoidAsync("preventDefaultOnmousedown", Ref);
            }

            await base.OnAfterRenderAsync(firstRender);
        }
    }
}
