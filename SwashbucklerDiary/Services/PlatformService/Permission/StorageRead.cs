namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        public async Task<bool> CheckStorageReadPermission()
        {
            return await CheckPermission<Permissions.StorageRead>();
        }

        public async Task<bool> TryStorageReadPermission()
        {
            return await TryPermission<Permissions.StorageRead>();
        }
    }
}
