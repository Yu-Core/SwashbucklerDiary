using Masa.Blazor;
using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public class CustomMTextarea : MTextarea
    {
        [Parameter]
        public EventCallback OnAfter { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                if (OnAfter.HasDelegate)
                {
                    await OnAfter.InvokeAsync();
                }
            }
        }
    }
}
