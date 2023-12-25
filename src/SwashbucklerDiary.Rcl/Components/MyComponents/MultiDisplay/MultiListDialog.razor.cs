using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MultiListDialog : DialogComponentBase
    {
        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        public List<DynamicListItem> DynamicListItems { get; set; } = new();

        private async Task OnClick(EventCallback callback)
        {
            await InternalVisibleChanged(false);
            await callback.InvokeAsync();
        }
    }
}
