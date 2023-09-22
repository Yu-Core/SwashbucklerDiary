using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;

namespace SwashbucklerDiary.Components
{
    public class BackupsPageComponentBase : ImportantComponentBase
    {
        [Inject]
        protected IAppDataService AppDataService { get; set; } = default!;
        [Inject]
        protected IDiaryService DiaryService { get; set; } = default!;

        protected async Task<bool> CheckPermissions()
        {
            var writePermission = await PlatformService.TryStorageWritePermission();
            if (!writePermission)
            {
                await AlertService.Info(I18n.T("Permission.OpenStorageWrite"));
                return false;
            }

            var readPermission = await PlatformService.TryStorageReadPermission();
            if (!readPermission)
            {
                await AlertService.Info(I18n.T("Permission.OpenStorageRead"));
                return false;
            }

            return true;
        }

        protected string SaveFileName()
        {
            string name = "SwashbucklerDiaryBackups";
            string time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string version = $"v{PlatformService.GetAppVersion()}";
            string suffix = ".db3";
            
            return name + time + version + suffix;
        }
    }
}
