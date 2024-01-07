using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class LocationCard : MyComponentBase
    {
        private bool ShowMenu;

        private List<DynamicListItem> MenuItems = new();

        [CascadingParameter]
        public LocationCardList LocationCardList { get; set; } = default!;

        [Parameter]
        public LocationModel Value { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LoadView();
        }

        private Task Delete()
        {
            return LocationCardList.Delete(Value);
        }

        private Task Rename()
        {
            return LocationCardList.Rename(Value);
        }

        void LoadView()
        {
            MenuItems = new()
            {
                new(this, "Share.Rename","mdi-rename-outline",Rename),
                new(this, "Share.Delete","mdi-delete-outline",Delete),
            };
        }
    }
}
