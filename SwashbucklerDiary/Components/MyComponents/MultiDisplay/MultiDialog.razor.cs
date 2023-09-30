using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class MultiDialog : DialogComponentBase
    {
        [Parameter]
        public string? Title { get; set; }

        [Parameter]
        public List<DynamicListItem> DynamicListItems { get; set; } = new();

        private async Task OnClick(EventCallback callback)
        {
            await InternalValueChanged(false);
            await callback.InvokeAsync();
        }
    }
}
