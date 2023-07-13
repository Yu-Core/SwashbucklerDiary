namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        public async Task<bool> CheckStorageWritePermission()
        {
            return await CheckPermission<Permissions.StorageWrite>();
        }

        public async Task<bool> TryStorageWritePermission()
        {
            return await TryPermission<Permissions.StorageWrite>();
        }
    }
}
