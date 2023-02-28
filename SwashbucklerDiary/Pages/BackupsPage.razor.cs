using BlazorComponent;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Config;

namespace SwashbucklerDiary.Pages
{
    public partial class BackupsPage : PageComponentBase
    {
        private StringNumber tabs = 0;
        private bool ShowBackups;
        private string? BackupsFolderPath;

        protected override async Task OnInitializedAsync()
        {
            await SetBackupsFolderPath();
            await base.OnInitializedAsync();
        }

        private async Task SetBackupsFolderPath()
        {
            BackupsFolderPath = await SettingsService.Get("BackupsPath", string.Empty);
            if (string.IsNullOrEmpty(BackupsFolderPath))
            {
                return;
            }

            if (!Directory.Exists(BackupsFolderPath))
            {
                BackupsFolderPath = string.Empty;
                await SettingsService.Save("BackupsPath", BackupsFolderPath);
            }
        }

        private async Task Backups()
        {
            ShowBackups = false;
            var writePermission = await SystemService.CheckStorageWritePermission();
            if (!writePermission)
            {
                await PopupService.ToastErrorAsync(I18n.T("Permission.OpenStorageWrite"));
                return;
            }

            var folderPath = await SettingsService.Get("BackupsPath", string.Empty);
            bool flag = false;
            if (string.IsNullOrEmpty(folderPath))
            {
                flag = true;
            }
            else
            {
                if (!Directory.Exists(folderPath))
                {
                    flag = true;
                    await PopupService.ToastErrorAsync("Invalid backup folder");
                }
            }
            
            if (flag)
            {
                folderPath = await SystemService.PickFolderAsync();
                if (!Directory.Exists(folderPath))
                {
                    return;
                }

                folderPath = Path.Combine(folderPath, "SwashbucklerDiaryBackups");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                BackupsFolderPath = folderPath;
                await SettingsService.Save("BackupsPath", folderPath);
            }

            var sourceFile = SQLiteConstants.DatabasePath;
            if (!File.Exists(sourceFile))
            {
                return;
            }

            var destFileName = "SwashbucklerDiaryBackups" +
                DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") +
                SystemService.GetAppVersion() + ".db3";
            var destFile = Path.Combine(folderPath, destFileName);
            File.Copy(sourceFile, destFile, true);
            await PopupService.ToastSuccessAsync(I18n.T("Backups.BackupsSuccess"));
        }

        private string? GetLocalPointer()
        {
            if (string.IsNullOrEmpty(BackupsFolderPath))
            {
                return I18n.T("Backups.SelectFolder");
            }
            else
            {
                return I18n.T("Backups.LocalPointer") + BackupsFolderPath;
            }
        }

        private void Restore()
        {

        }
    }
}
