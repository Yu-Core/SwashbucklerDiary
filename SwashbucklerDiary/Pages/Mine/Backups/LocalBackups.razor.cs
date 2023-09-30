using SwashbucklerDiary.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class LocalBackups : BackupsPageComponentBase
    {
        private bool ShowBackups;

        private bool ShowRestore;

        private string? BackupsFolderPath;

        private string? RestoreFilePath;

        protected override async Task OnInitializedAsync()
        {
            await SetBackupsFolderPath();
            await base.OnInitializedAsync();
        }

        private async Task SetBackupsFolderPath()
        {
            BackupsFolderPath = await SettingsService.Get(SettingType.BackupsPath);
        }

        private async Task Backups()
        {
            ShowBackups = false;
            var flag = await CheckPermissions();
            if (!flag)
            {
                return;
            }

            BackupsFolderPath = await SettingsService.Get(SettingType.BackupsPath);
            var diaries = await DiaryService.QueryAsync();
            var filePath = await AppDataService.BackupDatabase(BackupsFolderPath, diaries, true);
            if (filePath == null)
            {
                return;
            }

            BackupsFolderPath = Path.GetDirectoryName(filePath);
            await SettingsService.Save(SettingType.BackupsPath, BackupsFolderPath);
            await AlertService.Success(I18n.T("Backups.Backups.Success"));
        }

        private string? GetLocalBackupsDescription()
        {
            if (string.IsNullOrEmpty(BackupsFolderPath))
            {
                return I18n.T("Backups.Backups.SelectFolder");
            }
            else
            {
                return I18n.T("Backups.Backups.Content") + BackupsFolderPath;
            }
        }

        private async Task Restore()
        {
            var flag = await CheckPermissions();
            if (!flag)
            {
                return;
            }

            RestoreFilePath = string.Empty;
            RestoreFilePath = await PlatformService.PickZipFileAsync();
            if (string.IsNullOrEmpty(RestoreFilePath))
            {
                return;
            }
            ShowRestore = true;
        }

        private async Task ConfirmRestore()
        {
            ShowRestore = false;
            if (string.IsNullOrEmpty(RestoreFilePath))
            {
                await AlertService.Error(I18n.T("Backups.Restore.Fail"));
                return;
            }

            bool flag = await AppDataService.RestoreDatabase(RestoreFilePath);
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
