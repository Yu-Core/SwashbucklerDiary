using SwashbucklerDiary.Components;

namespace SwashbucklerDiary.Pages
{
    public partial class PermissionPage : PageComponentBase,IDisposable
    {
        private bool CameraState;
        private bool StorageState;
        private string? CameraPermission => GetPermissionText(CameraState);
        private string? StoragePermission => GetPermissionText(StorageState);

        public void Dispose()
        {
            SystemService.Resumed -= UpdatePermissionStates;
            GC.SuppressFinalize(this);
        }

        protected override async Task OnInitializedAsync()
        {
            await UpdatePermissionStatesAsync();
            SystemService.Resumed += UpdatePermissionStates;
            base.OnInitialized();
        }

        private void OpenSystemPermissionSetting()
        {
            SystemService.OpenSystemSetting();
        }

        private async void UpdatePermissionStates()
        {
            await UpdatePermissionStatesAsync();
            StateHasChanged();
        }

        private async Task UpdatePermissionStatesAsync()
        {
            CameraState = await SystemService.CheckCameraPermission();
            var readStorageState = await SystemService.CheckStorageReadPermission();
            var writeStorageState = await SystemService.CheckStorageWritePermission();
            StorageState = readStorageState || writeStorageState;
        }

        private string GetPermissionText(bool value)
        {
            return value ? I18n.T("Permission.Enable") : I18n.T("Permission.Disable");
        }
    }
}
