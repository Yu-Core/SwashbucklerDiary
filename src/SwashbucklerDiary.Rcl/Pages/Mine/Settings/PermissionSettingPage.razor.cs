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

            AppLifecycle.OnResumed += UpdatePermissionStates;
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

            AppLifecycle.OnResumed -= UpdatePermissionStates;
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
            await InvokeAsync(StateHasChanged);
        }

        private async Task UpdatePermissionStatesAsync()
        {
            cameraState = await PlatformIntegration.CheckCameraPermission();
            storageState = await PlatformIntegration.CheckStorageWritePermission();
        }

        private string GetPermissionText(bool value)
        {
            return value ? I18n.T("Granted") : I18n.T("Not granted");
        }
    }
}
