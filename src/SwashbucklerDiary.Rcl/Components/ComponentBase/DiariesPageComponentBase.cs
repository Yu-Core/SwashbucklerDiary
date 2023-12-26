using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public class DiariesPageComponentBase : ImportantComponentBase
    {
        protected bool showDiarySort;

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

        protected override async Task OnResume()
        {
            await UpdateDiariesAsync();
            await UpdateTagsAsync();
            await base.OnResume();
        }
    }
}
