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
            await base.OnInitializedAsync();
            await InitDiaries();
            await UpdateTags();
        }

        protected virtual Task InitDiaries()
        {
            return UpdateDiaries();
        }

        protected virtual async Task UpdateDiaries()
        {
            Diaries = await DiaryService.QueryAsync(it => !it.Private);
        }

        protected virtual async Task UpdateTags()
        {
            Tags = await TagService.QueryAsync();
        }
    }
}
