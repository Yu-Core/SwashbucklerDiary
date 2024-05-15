using BlazorComponent;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MultiMenu : DialogComponentBase
    {
        private List<DynamicListItem> previousDynamicListItems = [];

        [Parameter]
        public RenderFragment<ActivatorProps> ActivatorContent { get; set; } = default!;

        [Parameter]
        public List<DynamicListItem> DynamicListItems { get; set; } = [];

        public Dictionary<string, object> ActivatorAttributes { get; set; } = [];

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (previousDynamicListItems != DynamicListItems)
            {
                DynamicListItems.ForEach(item =>
                {
                    var onClick = item.OnClick;
                    item.OnClick = EventCallback.Factory.Create(this, () => HandleOnContentClick(onClick));
                });
                previousDynamicListItems = DynamicListItems;
            }
        }

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

        private async Task HandleOnContentClick(EventCallback eventCallback)
        {
            await InternalVisibleChanged(false);
            _ = eventCallback.InvokeAsync();
        }
    }
}
