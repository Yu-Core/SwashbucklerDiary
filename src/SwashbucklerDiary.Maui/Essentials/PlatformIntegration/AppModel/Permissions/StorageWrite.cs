namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        public async Task<bool> CheckStorageWritePermission()
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                return true;
            }

            return await CheckPermission<Permissions.StorageWrite>().ConfigureAwait(false);
        }

        public async Task<bool> TryStorageWritePermission()
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                return true;
            }

            return await TryPermission<Permissions.StorageWrite>().ConfigureAwait(false);
        }
    }
}
