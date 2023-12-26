using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class LocalBackups : ImportantComponentBase
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
                return;
            }

            string filePath = await DiaryFileManager.ExportDBAsync(true);
            string name = await DiaryFileManager.GetBackupFileName();
            bool isSuccess = await PlatformIntegration.SaveFileAsync(name, filePath);
            if (!isSuccess)
            {
                return;
            }

            await AlertService.Success(I18n.T("Backups.Backups.Success"));
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
            if (string.IsNullOrEmpty(restoreFilePath))
            {
                await AlertService.Error(I18n.T("Backups.Restore.Fail"));
                return;
            }

            bool flag = await DiaryFileManager.ImportDBAsync(restoreFilePath);
            if (flag)
            {
                await AlertService.Success(I18n.T("Backups.Restore.Success"));
            }
            else
            {
                await AlertService.Error(I18n.T("Backups.Restore.Fail"));
            }
        }
    }

}
