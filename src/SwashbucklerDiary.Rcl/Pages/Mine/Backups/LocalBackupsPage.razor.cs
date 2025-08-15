using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class LocalBackupsPage : ImportantComponentBase
    {
        private bool showBackups;

        private bool showRestore;

        private string? restoreFilePath;

        [Inject]
        private IDiaryFileManager DiaryFileManager { get; set; } = default!;

        private async Task Backups()
        {
            showBackups = false;
            var flag = await PlatformIntegration.TryStorageWritePermission();
            if (!flag)
            {
                await AlertService.InfoAsync(I18n.T("Please grant permission for storage writing"));
                return;
            }

            AlertService.StartLoading();

            try
            {
                string filePath = await DiaryFileManager.ExportDBAsync(true);
                string name = DiaryFileManager.GetBackupFileName();
                bool isSuccess = await PlatformIntegration.SaveFileAsync(name, filePath);
                if (isSuccess)
                {
                    await AlertService.SuccessAsync(I18n.T("Backup successful"));
                }
            }
            finally
            {
                AlertService.StopLoading();
            }
        }

        private async Task Restore()
        {
            restoreFilePath = string.Empty;
            restoreFilePath = await PlatformIntegration.PickZipFileAsync();
            if (string.IsNullOrEmpty(restoreFilePath))
            {
                return;
            }

            showRestore = true;
        }

        private async Task ConfirmRestore()
        {
            showRestore = false;
            StateHasChanged();

            if (string.IsNullOrEmpty(restoreFilePath))
            {
                await AlertService.ErrorAsync(I18n.T("Restore failed"));
                return;
            }

            AlertService.StartLoading();

            try
            {
                bool flag = await DiaryFileManager.ImportDBAsync(restoreFilePath);
                if (flag)
                {
                    await AlertService.SuccessAsync(I18n.T("Restore successfully"));
                }
                else
                {
                    await AlertService.ErrorAsync(I18n.T("Restore failed"));
                }
            }
            finally
            {
                AlertService.StopLoading();
            }
        }
    }

}
