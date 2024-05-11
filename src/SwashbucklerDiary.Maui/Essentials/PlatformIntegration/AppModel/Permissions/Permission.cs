namespace SwashbucklerDiary.Maui.Essentials
{
    public partial class PlatformIntegration
    {
        public static event Action<Type>? BeforeRequestPermissionAsync;

        public static event Action? AfterRequestPermissionAsync;

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

            return await RequestPermissionAsync<T>();
        }

        private static async Task<bool> RequestPermissionAsync<T>() where T : Permissions.BasePermission, new()
        {
            BeforeRequestPermissionAsync?.Invoke(typeof(T));

            var status = await Permissions.RequestAsync<T>();

            AfterRequestPermissionAsync?.Invoke();

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
