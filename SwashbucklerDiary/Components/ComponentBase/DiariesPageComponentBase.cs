using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public class DiariesPageComponentBase : PageComponentBase
    {
        [Inject]
        protected IDiaryService DiaryService { get; set; } = default!;
        [Inject]
        protected ITagService TagService { get; set; } = default!;

        protected virtual List<DiaryModel> Diaries { get; set; } = new();
        protected virtual List<TagModel> Tags { get; set; } = new();

        protected override async Task OnInitializedAsync()
        {
            await InitializeDiariesAsync();
            await UpdateTagsAsync();
            await base.OnInitializedAsync();
        }

        protected virtual Task InitializeDiariesAsync()
        {
            return UpdateDiariesAsync();
        }

        protected virtual async Task UpdateDiariesAsync()
        {
            Diaries = await DiaryService.QueryAsync(it => !it.Private);
        }

        protected virtual async Task UpdateTagsAsync()
        {
            Tags = await TagService.QueryAsync();
        }
    }
}
