using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class MyMenu : DialogComponentBase
    {
        [Parameter]
        public RenderFragment? ButtonContent { get; set; }
        [Parameter]
        public List<ViewListItem> ViewListItems { get; set; } = new();

        private async Task OnClick(Action action)
        {
            await InternalValueChanged(false);
            action.Invoke();
        }
    }
}
