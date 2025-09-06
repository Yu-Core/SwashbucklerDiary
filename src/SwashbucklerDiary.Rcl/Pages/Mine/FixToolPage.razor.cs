using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class FixToolPage
    {
        [Inject]
        private IDiaryFileManager DiaryFileManager { get; set; } = default!;

        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;

        private async Task FixEarlyImagesNotDisplay()
        {
            AlertService.StartLoading("正在修复");
            try
            {
                await DiaryFileManager.AllUseNewResourceUriAsync();
                await AlertService.SuccessAsync("修复完成");
            }
            finally
            {
                AlertService.StopLoading();
            }
        }

        private async Task FixPrivacyModeDisplayError()
        {
            AlertService.StartLoading(I18n.T("Fixing in progress"));
            try
            {
                await DiaryService.MovePrivacyDiariesAsync();
                await AlertService.SuccessAsync(I18n.T("Fix completed"));
            }
            finally
            {
                AlertService.StopLoading();
            }
        }

        private async Task FixAudioVdieoRecord()
        {
            AlertService.StartLoading(I18n.T("Fixing in progress"));
            try
            {
                await DiaryFileManager.UpdateAllDiariesResourcesAsync();
                await AlertService.SuccessAsync(I18n.T("Fix completed"));
            }
            finally
            {
                AlertService.StopLoading();
            }
        }
    }
}