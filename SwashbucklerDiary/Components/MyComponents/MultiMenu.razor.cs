using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class MultiMenu : DialogComponentBase
    {
        [Parameter]
        public RenderFragment? ButtonContent { get; set; }
        [Parameter]
        public List<ListItemModel> ListItemModels { get; set; } = new();

        private async Task OnClick(MulticastDelegate @delegate)
        {
            await InternalValueChanged(false);
            @delegate.DynamicInvoke();
        }
    }
}
