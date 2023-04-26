using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class MultiDialog : DialogComponentBase
    {
        [Parameter]
        public string? Title { get; set; }
        [Parameter]
        public List<ListItemModel> ListItemModels { get; set; } = new();

        private async Task OnClick(MulticastDelegate @delegate)
        {
            await InternalValueChanged(false);
            @delegate.DynamicInvoke();
        }
    }
}
