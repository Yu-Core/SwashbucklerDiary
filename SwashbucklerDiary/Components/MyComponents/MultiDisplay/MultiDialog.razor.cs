using Microsoft.AspNetCore.Components;
using OneOf;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class MultiDialog : DialogComponentBase
    {
        [Parameter]
        public string? Title { get; set; }
        [Parameter]
        public List<DynamicListItem> ListItemModels { get; set; } = new();

        private async Task OnClick(EventCallback callback)
        {
            await InternalValueChanged(false);
            await callback.InvokeAsync();
        }
    }
}
