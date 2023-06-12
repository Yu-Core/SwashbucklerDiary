using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class LocationCard : MyComponentBase
    {
        private bool ShowMenu;
        private List<DynamicListItem> ListItemModels = new();

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
                new(this, "Share.Rename","mdi-rename-outline",()=>OnRename.InvokeAsync(Value)),
                new(this, "Share.Delete","mdi-delete-outline",()=>OnDelete.InvokeAsync(Value)),
            };
        }
    }
}
