namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        public async Task<bool> CheckStorageWritePermission()
        {
            if(OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                return true;
            }

            return await CheckPermission<Permissions.StorageWrite>();
        }

        public async Task<bool> TryStorageWritePermission()
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                return true;
            }

            return await TryPermission<Permissions.StorageWrite>("Permission.OpenStorageWrite");
        }
    }
}
