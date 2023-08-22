using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class TagCard : MyComponentBase
    {
        private bool ShowMenu;
        private List<DynamicListItem> ListItemModels = new();

        [Parameter]
        public TagModel? Value { get; set; }
        [Parameter]
        public EventCallback<TagModel> OnDelete { get; set; }
        [Parameter]
        public EventCallback<TagModel> OnRename { get; set; }
        [Parameter]
        public EventCallback<TagModel> OnClick { get; set; }

        protected override void OnInitialized()
        {
            LoadView();
            base.OnInitialized();
        }

        private string DiaryCount
        {
            get
            {
                if(Value?.Diaries is null || Value.Diaries.Count == 0)
                {
                    return string.Empty;
                }

                return Value.Diaries.Count.ToString();
            }
        }

        void LoadView()
        {
            ListItemModels = new()
            {
                new(this, "Share.Rename","mdi-rename-outline",() => OnRename.InvokeAsync(Value)),
                new(this, "Share.Delete","mdi-delete-outline",() => OnDelete.InvokeAsync(Value)),
            };
        }
    }
}
