using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class TagCard : MyComponentBase
    {
        private bool ShowMenu;
        private List<DynamicListItem> ListItemModels = new();

        [CascadingParameter]
        public TagCardList TagCardList { get; set; } = default!;
        [Parameter]
        public TagModel Value { get; set; } = default!;

        protected override void OnInitialized()
        {
            LoadView();
            base.OnInitialized();
        }

        private string DiaryCount
        {
            get
            {
                var count = TagCardList.GetDiaryCount(Value);
                return count == 0 ? string.Empty : count.ToString();
            }
        }

        private Task Delete() => TagCardList.Delete(Value);
        private Task Rename() => TagCardList.Rename(Value);

        private void LoadView()
        {
            ListItemModels = new()
            {
                new(this, "Share.Rename","mdi-rename-outline",Rename),
                new(this, "Share.Delete","mdi-delete-outline",Delete),
            };
        }

        private void ToTagPage()
        {
            NavigateService.NavigateTo($"/tag/{Value.Id}");
        }
    }
}
