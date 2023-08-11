using SwashbucklerDiary.Components;

namespace SwashbucklerDiary.Pages
{
    public partial class PermissionSettingPage : PageComponentBase,IDisposable
    {
        private bool CameraState;
        private bool StorageState;
        private string? CameraPermission => GetPermissionText(CameraState);
        private string? StoragePermission => GetPermissionText(StorageState);

        public void Dispose()
        {
            PlatformService.Resumed -= UpdatePermissionStates;
            GC.SuppressFinalize(this);
        }

        protected override async Task OnInitializedAsync()
        {
            await UpdatePermissionStatesAsync();
            PlatformService.Resumed += UpdatePermissionStates;
            base.OnInitialized();
        }

        private void OpenSystemPermissionSetting()
        {
            PlatformService.OpenPlatformSetting();
        }

        private async void UpdatePermissionStates()
        {
            await UpdatePermissionStatesAsync();
            StateHasChanged();
        }

        private async Task UpdatePermissionStatesAsync()
        {
            CameraState = await PlatformService.CheckCameraPermission();
            var readStorageState = await PlatformService.CheckStorageReadPermission();
            var writeStorageState = await PlatformService.CheckStorageWritePermission();
            StorageState = readStorageState || writeStorageState;
        }

        private string GetPermissionText(bool value)
        {
            return value ? I18n.T("Permission.Enable") : I18n.T("Permission.Disable");
        }
    }
}
