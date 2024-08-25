using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class TagCard : CardComponentBase<TagModel>
    {
        private string? diaryCount;

        [CascadingParameter]
        public TagCardListOptions TagCardListOptions { get; set; } = default!;

        [EditorRequired]
        [Parameter]
        public Func<TagModel, int>? OnCalcDiaryCount { get; set; }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            TagCardListOptions.DiariesChanged -= UpdateDiaryCount;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                UpdateDiaryCount();
                TagCardListOptions.DiariesChanged += UpdateDiaryCount;
            }
        }

        private void UpdateDiaryCount()
        {
            if (OnCalcDiaryCount is not null)
            {
                var count = OnCalcDiaryCount.Invoke(Value);
                diaryCount = count == 0 ? string.Empty : count.ToString();
                InvokeAsync(StateHasChanged);
            }
        }

        private void ToTagPage()
        {
            NavigationManager.NavigateTo($"tagDetails/{Value.Id}");
        }
    }
}
