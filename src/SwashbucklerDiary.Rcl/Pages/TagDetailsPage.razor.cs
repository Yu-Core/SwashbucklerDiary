using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class TagDetailsPage : DiariesPageComponentBase
    {
        private string? tagName;

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
            var tag = await TagService.FindIncludesAsync(Id);
            if (tag is null)
            {
                await NavigateToBack();
                return;
            }

            tagName = tag?.Name;
            Diaries = tag?.Diaries ?? [];
        }

        private void ToWrite()
        {
            NavigationManager.NavigateTo($"write?tagId={Id}");
        }
    }
}
