using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class ShareDialog : DialogComponentBase
    {
        [Parameter]
        public List<ListItemModel> Items { get; set; } = new();
    }
}
