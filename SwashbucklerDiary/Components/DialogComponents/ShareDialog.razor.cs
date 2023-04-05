using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class ShareDialog : DialogComponentBase
    {
        [Parameter]
        public List<ViewListItem> Items { get; set; } = new();
    }
}
