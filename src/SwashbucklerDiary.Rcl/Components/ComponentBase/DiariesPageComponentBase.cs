using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public abstract class DiariesPageComponentBase : ImportantComponentBase
    {
        [Inject]
        protected IDiaryService DiaryService { get; set; } = default!;

        [Inject]
        protected ITagService TagService { get; set; } = default!;

        protected virtual List<DiaryModel> Diaries { get; set; } = [];

        protected virtual List<TagModel> Tags { get; set; } = [];

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await Task.WhenAll(
                    InitializeDiariesAsync(),
                    UpdateTagsAsync());
                StateHasChanged();
            }
        }

        protected virtual Task InitializeDiariesAsync()
        {
            return UpdateDiariesAsync();
        }

        protected virtual async Task UpdateDiariesAsync()
        {
            Diaries = await DiaryService.QueryDiariesAsync();
        }

        protected virtual async Task UpdateTagsAsync()
        {
            Tags = await TagService.QueryAsync();
        }

        protected override async Task OnResume()
        {
            await Task.WhenAll(
                UpdateDiariesAsync(),
                UpdateTagsAsync());
            await base.OnResume();
        }
    }
}
