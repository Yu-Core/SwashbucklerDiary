using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class TagCard : CardComponentBase<TagModel>
    {
        [CascadingParameter]
        public TagCardList TagCardList { get; set; } = default!;

        private string DiaryCount
        {
            get
            {
                var count = TagCardList.GetDiaryCount(Value);
                return count == 0 ? string.Empty : count.ToString();
            }
        }

        private void ToTagPage()
        {
            NavigateService.PushAsync($"tag?Id={Value.Id}");
        }
    }
}
