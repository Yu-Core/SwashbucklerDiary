using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class TagCard : MyComponentBase
    {
        private bool ShowMenu;

        private List<DynamicListItem> MenuItems = new();

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

        private Task Delete()
        {
            ShowMenu = false;
            return TagCardList.Delete(Value);
        }

        private Task Rename()
        {
            ShowMenu = false;
            return TagCardList.Rename(Value);
        }

        private Task Export()
        {
            ShowMenu = false;
            return TagCardList.Export(Value);
        }

        private void LoadView()
        {
            MenuItems = new()
            {
                new(this, "Share.Rename","mdi-rename-outline",Rename),
                new(this, "Share.Delete","mdi-delete-outline",Delete),
                new(this, "Diary.Export","mdi-export",Export),
            };
        }

        private void ToTagPage()
        {
            NavigateService.PushAsync($"tag/{Value.Id}");
        }
    }
}
