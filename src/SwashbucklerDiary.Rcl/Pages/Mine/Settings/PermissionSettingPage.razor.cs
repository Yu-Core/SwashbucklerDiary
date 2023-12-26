using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class PermissionSettingPage : ImportantComponentBase
    {
        private bool cameraState;

        private bool storageState;

        [Inject]
        private IAppLifecycle AppLifecycle { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await UpdatePermissionStatesAsync();
            AppLifecycle.Resumed += UpdatePermissionStates;
            base.OnInitialized();
        }

        protected override void OnDispose()
        {
            AppLifecycle.Resumed -= UpdatePermissionStates;
            base.OnDispose();
        }

        private string? CameraPermission => GetPermissionText(cameraState);

        private string? StoragePermission => GetPermissionText(storageState);

        private void OpenSystemPermissionSetting()
        {
            PlatformIntegration.ShowSettingsUI();
        }

        private async void UpdatePermissionStates()
        {
            await UpdatePermissionStatesAsync();
            StateHasChanged();
        }

        private async Task UpdatePermissionStatesAsync()
        {
            cameraState = await PlatformIntegration.CheckCameraPermission();
            storageState = await PlatformIntegration.CheckStorageWritePermission();
        }

        private string GetPermissionText(bool value)
        {
            return value ? I18n.T("Permission.Enable") : I18n.T("Permission.Disable");
        }
    }
}
