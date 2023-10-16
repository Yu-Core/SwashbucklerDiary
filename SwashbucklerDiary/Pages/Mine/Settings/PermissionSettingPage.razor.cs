using SwashbucklerDiary.Components;

namespace SwashbucklerDiary.Pages
{
    public partial class PermissionSettingPage : ImportantComponentBase
    {
        private bool CameraState;

        private bool StorageState;

        protected override async Task OnInitializedAsync()
        {
            await UpdatePermissionStatesAsync();
            PlatformService.Resumed += UpdatePermissionStates;
            base.OnInitialized();
        }

        protected override void OnDispose()
        {
            PlatformService.Resumed -= UpdatePermissionStates;
            base.OnDispose();
        }

        private string? CameraPermission => GetPermissionText(CameraState);

        private string? StoragePermission => GetPermissionText(StorageState);

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
            StorageState = await PlatformService.CheckStorageWritePermission();
        }

        private string GetPermissionText(bool value)
        {
            return value ? I18n.T("Permission.Enable") : I18n.T("Permission.Disable");
        }
    }
}
