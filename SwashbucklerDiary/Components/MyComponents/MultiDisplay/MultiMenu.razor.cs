using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class MultiMenu : DialogComponentBase
    {
        [Parameter]
        public RenderFragment? ButtonContent { get; set; }
        [Parameter]
        public List<DynamicListItem> ListItemModels { get; set; } = new();

        private async Task OnClick(EventCallback callback)
        {
            await InternalValueChanged(false);
            await callback.InvokeAsync();
        }
    }
}
