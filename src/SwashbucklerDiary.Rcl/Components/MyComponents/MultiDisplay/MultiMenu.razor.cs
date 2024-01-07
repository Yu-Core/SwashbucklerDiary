using BlazorComponent;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MultiMenu : DialogComponentBase
    {
        [EditorRequired]
        [Parameter]
        public RenderFragment<ActivatorProps> ActivatorContent { get; set; } = default!;

        [Parameter]
        public List<DynamicListItem> DynamicListItems { get; set; } = [];

        private async Task UpdateDisplay(bool value)
        {
            if (Visible)
            {
                await InternalVisibleChanged(false);
            }
        }

    }
}
