using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class PrivacyLibraryPage : PageComponentBase
    {
        private List<DiaryModel> Diaries = new();

        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await UpdateDiaries();
            await base.OnInitializedAsync();
        }

        private async Task UpdateDiaries()
        {
            Diaries = await DiaryService.QueryAsync(it => it.Private);
        }
    }
}
