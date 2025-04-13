using Microsoft.AspNetCore.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class ReferencedDetails
    {
        private readonly string scrollContainerId = $"scroll-container-{Guid.NewGuid():N}";

        private string scrollContainerSelector = string.Empty;

        [Parameter]
        public Guid Id { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            scrollContainerSelector = $"#{scrollContainerId}";
        }

        protected override async Task UpdateDiariesAsync()
        {
            string referencedText = $"read/{Id}";
            Diaries = await DiaryService.QueryAsync(it => (it.Content ?? string.Empty).Contains(referencedText ?? string.Empty, StringComparison.CurrentCultureIgnoreCase));
        }
    }
}