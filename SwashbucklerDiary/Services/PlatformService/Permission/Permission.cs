namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        private static async Task<bool> TryPermission<T>() where T : Permissions.BasePermission, new()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<T>();

            if (status == PermissionStatus.Granted)
                return true;

            if (status == PermissionStatus.Denied)
            {
                if (DeviceInfo.Platform == DevicePlatform.iOS ||
                    DeviceInfo.Platform == DevicePlatform.macOS ||
                    DeviceInfo.Platform == DevicePlatform.MacCatalyst)
                    return false;
            }

            status = await Permissions.RequestAsync<T>();

            if (status == PermissionStatus.Granted)
                return true;

            if (status == PermissionStatus.Denied)
            {
                return false;
            }

            return true;
        }

        private static async Task<bool> CheckPermission<T>() where T : Permissions.BasePermission, new()
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<T>();

            if (status == PermissionStatus.Granted)
                return true;
            return false;
        }
    }
}
