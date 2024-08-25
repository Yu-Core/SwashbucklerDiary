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

        protected override void OnInitialized()
        {
            base.OnInitialized();

            AppLifecycle.Resumed += UpdatePermissionStates;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await UpdatePermissionStatesAsync();
                StateHasChanged();
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            AppLifecycle.Resumed -= UpdatePermissionStates;
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
