using SwashbucklerDiary.Config;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class BackupsLocal : BackupsPageComponentBase
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

            var sourceFile = SQLiteConstants.DatabasePath;
            if (!File.Exists(sourceFile))
            {
                await AlertService.Alert(I18n.T("Backups.NoDbFile"));
                return;
            }

            var destFileName = SaveFileName();

            //There is a bug in the file saved in the pick folder by Android
            //https://github.com/dotnet/maui/issues/5295
            //https://learn.microsoft.com/en-us/answers/questions/1183152/open-a-file-get-its-path-save-the-file-maui-androi

            using var stream = File.OpenRead(sourceFile);
            var filePath = await SystemService.SaveFileAsync(BackupsFolderPath, destFileName, stream);
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
            RestoreFilePath = await SystemService.PickDBFileAsync();
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
                await AlertService.Success(I18n.T("Backups.Restore.Fail"));
                return;
            }

            File.Copy(RestoreFilePath, SQLiteConstants.DatabasePath, true);
            await AlertService.Success(I18n.T("Backups.Restore.Success"));
        }
    }

}
