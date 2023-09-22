using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;

namespace SwashbucklerDiary.Pages
{
    public partial class TagPage : DiariesPageComponentBase
    {
        private string? TagName;

        [Parameter]
        public Guid Id { get; set; }

        protected override async Task UpdateDiariesAsync()
        {
            var tag = await TagService.FindIncludesAsync(Id);
            TagName = tag?.Name;
            Diaries = tag?.Diaries ?? new();
        }


        private void NavigateToWrite()
        {
            NavigateService.PushAsync($"/write?tagId={Id}");
        }
    }
}
