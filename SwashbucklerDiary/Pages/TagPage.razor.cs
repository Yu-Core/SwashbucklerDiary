using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class TagPage : DiariesPageComponentBase
    {
        private TagModel Tag = new();

        [Parameter]
        public Guid Id { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var tagModel = await TagService.FindIncludesAsync(Id);
            if (tagModel == null)
            {
                NavigateToBack();
                return;
            }
            Tag = tagModel;
            await base.OnInitializedAsync();
        }

        protected override Task UpdateDiariesAsync()
        {
            Diaries = Tag.Diaries ?? new();
            return Task.CompletedTask;
        }


        private void NavigateToWrite()
        {
            NavigateService.NavigateTo($"/write?tagId={Id}");
        }
    }
}
