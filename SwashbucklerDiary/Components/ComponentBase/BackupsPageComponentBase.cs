namespace SwashbucklerDiary.Components
{
    public class BackupsPageComponentBase : PageComponentBase
    {
        protected async Task<bool> CheckPermission()
        {
            var readPermission = await SystemService.TryStorageReadPermission();
            if (!readPermission)
            {
                await AlertService.Success(I18n.T("Permission.OpenStorageRead"));
                return false;
            }

            var writePermission = await SystemService.TryStorageWritePermission();
            if (!writePermission)
            {
                await AlertService.Error(I18n.T("Permission.OpenStorageWrite"));
                return false;
            }

            return true;
        }

        protected string SaveFileName()
        {
            var destFileName = "SwashbucklerDiaryBackups" +
                DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") +
                "v" +
                SystemService.GetAppVersion() + ".db3";
            return destFileName;
        }
    }
}
