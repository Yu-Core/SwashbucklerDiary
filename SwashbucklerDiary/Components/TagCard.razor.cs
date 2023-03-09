using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class TagCard : MyComponentBase
    {
        private bool ShowMenu;

        [Parameter]
        public TagModel? Value { get; set; }
        [Parameter]
        public EventCallback OnDelete { get; set; }
        [Parameter]
        public EventCallback OnRename { get; set; }
        [Parameter]
        public EventCallback OnClick { get; set; }

        private List<ViewListItem> ViewListItems => new()
        {
            new("Share.Rename","mdi-rename-outline",()=>OnRename.InvokeAsync()),
            new("Share.Delete","mdi-delete-outline",()=>OnDelete.InvokeAsync()),
        };
    }
}
