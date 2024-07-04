using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class TagDetailsPage : DiariesPageComponentBase
    {
        private string? tagName;

        [Parameter]
        public Guid Id { get; set; }

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

        private void NavigateToWrite()
        {
            NavigationManager.NavigateTo($"write?tagId={Id}");
        }
    }
}
