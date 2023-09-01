using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class LocationCard : MyComponentBase
    {
        private bool ShowMenu;
        private List<DynamicListItem> ListItemModels = new();

        [CascadingParameter]
        public LocationCardList LocationCardList { get; set; } = default!;
        [Parameter]
        public LocationModel Value { get; set; } = default!;

        protected override void OnInitialized()
        {
            LoadView();
            base.OnInitialized();
        }

        private Task Delete() => LocationCardList.Delete(Value);
        private Task Rename() => LocationCardList.Rename(Value);

        void LoadView()
        {
            ListItemModels = new()
            {
                new(this, "Share.Rename","mdi-rename-outline",Rename),
                new(this, "Share.Delete","mdi-delete-outline",Delete),
            };
        }
    }
}
