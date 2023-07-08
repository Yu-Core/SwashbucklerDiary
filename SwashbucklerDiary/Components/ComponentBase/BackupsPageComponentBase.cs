namespace SwashbucklerDiary.Components
{
    public class BackupsPageComponentBase : PageComponentBase
    {
        protected async Task<bool> CheckPermissions()
        {
            var writePermission = await SystemService.TryStorageWritePermission();
            if (!writePermission)
            {
                await AlertService.Error(I18n.T("Permission.OpenStorageWrite"));
                return false;
            }

            var readPermission = await SystemService.TryStorageReadPermission();
            if (!readPermission)
            {
                await AlertService.Success(I18n.T("Permission.OpenStorageRead"));
                return false;
            }

            return true;
        }

        protected string SaveFileName()
        {
            string name = "SwashbucklerDiaryBackups";
            string time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string version = $"v{SystemService.GetAppVersion()}";
            string suffix = ".db3";
            
            return name + time + version + suffix;
        }
    }
}
