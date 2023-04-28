using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class LocationCard : MyComponentBase
    {
        private bool ShowMenu;
        private List<ListItemModel> ListItemModels = new();

        [Parameter]
        public LocationModel? Value { get; set; }
        [Parameter]
        public EventCallback<LocationModel> OnDelete { get; set; }
        [Parameter]
        public EventCallback<LocationModel> OnRename { get; set; }

        protected override void OnInitialized()
        {
            LoadView();
            base.OnInitialized();
        }

        void LoadView()
        {
            ListItemModels = new()
            {
                new("Share.Rename","mdi-rename-outline",EC(()=>OnRename.InvokeAsync(Value))),
                new("Share.Delete","mdi-delete-outline",EC(()=>OnDelete.InvokeAsync(Value))),
            };
        }
    }
}
