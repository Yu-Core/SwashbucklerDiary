using BlazorComponent;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MultiMenu : DialogComponentBase
    {
        [Parameter]
        public RenderFragment<ActivatorProps> ActivatorContent { get; set; } = default!;

        [Parameter]
        public List<DynamicListItem> DynamicListItems { get; set; } = [];

        public Dictionary<string, object> ActivatorAttributes { get; set; } = [];

        protected RenderFragment? ComputedActivatorContent(ActivatorProps props)
        {
            ActivatorAttributes = props.Attrs;
            if (ActivatorContent is null)
            {
                return null;
            }

            return ActivatorContent(props);
        }

        private async Task UpdateDisplay(bool value)
        {
            if (Visible)
            {
                await InternalVisibleChanged(false);
            }
        }
    }
}
