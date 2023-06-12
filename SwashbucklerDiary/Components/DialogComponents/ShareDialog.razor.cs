using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class ShareDialog : DialogComponentBase
    {
        [Parameter]
        public List<DynamicListItem> Items { get; set; } = new();
    }
}
