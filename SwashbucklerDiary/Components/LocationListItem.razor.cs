using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class LocationListItem : MyComponentBase
    {
        private bool ShowMenu;

        [Parameter]
        public LocationModel? Value { get; set; }
        [Parameter]
        public EventCallback OnDelete { get; set; }
        [Parameter]
        public EventCallback OnRename { get; set; }

        private List<ViewListItem> ViewListItems => new()
        {
            new("Share.Rename","mdi-rename-outline",()=>OnRename.InvokeAsync()),
            new("Share.Delete","mdi-delete-outline",()=>OnDelete.InvokeAsync()),
        };
    }
}
