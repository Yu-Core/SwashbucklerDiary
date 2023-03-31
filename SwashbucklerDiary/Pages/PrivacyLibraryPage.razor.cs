using SwashbucklerDiary.Components;

namespace SwashbucklerDiary.Pages
{
    public partial class PrivacyLibraryPage : DiariesPageComponentBase
    {
        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }

        protected override async Task UpdateDiaries()
        {
            Diaries = await DiaryService.QueryAsync(it => it.Private);
        }
    }
}
