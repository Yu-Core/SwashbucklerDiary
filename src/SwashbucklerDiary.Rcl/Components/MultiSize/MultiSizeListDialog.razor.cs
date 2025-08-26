using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class MultiSizeListDialog : DialogComponentBase
    {
        [Parameter]
        public string? Title { get; set; }

        [EditorRequired]
        [Parameter]
        public List<DynamicListItem> DynamicListItems { get; set; } = [];
    }
}
