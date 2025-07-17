using Masa.Blazor.Presets;
using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Components
{
    public class CustomPPageContainer : PPageContainer
    {
        [Parameter]
        public bool PageUpdate { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            Strict = !PageUpdate;
        }
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            Strict = !PageUpdate;
        }
    }
}
