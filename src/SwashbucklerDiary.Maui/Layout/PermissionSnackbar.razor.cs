using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Maui.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Maui.Layout
{
    public partial class PermissionSnackbar : IDisposable
    {
        private bool showSnackbar;

        private string? title;

        private string? subTitle;

        private Dictionary<string, string> RequestPermissionTitles = new()
        {
            { nameof(Permissions.Camera), "Camera permission description" },
            { nameof(Permissions.StorageWrite), "StorageWrite permission description" }
        };

        private Dictionary<string, string> RequestPermissionDescriptions = new()
        {
            { nameof(Permissions.Camera), "Used for changing avatars, taking pictures, and other scenes" },
            { nameof(Permissions.StorageWrite), "Used for data backup, import and export, taking pictures, and other scenarios" }
        };

        [Inject]
        private II18nService I18n { get; set; } = default!;

        public void Dispose()
        {
            PlatformIntegration.BeforeRequestPermissionAsync -= OnShowSnackbar;
            PlatformIntegration.AfterRequestPermissionAsync -= OnCloseSnackbar;
            GC.SuppressFinalize(this);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            PlatformIntegration.BeforeRequestPermissionAsync += OnShowSnackbar;
            PlatformIntegration.AfterRequestPermissionAsync += OnCloseSnackbar;
        }

        private void OnShowSnackbar(Type type)
        {
            if (RequestPermissionTitles.TryGetValue(type.Name, out var titleValue))
            {
                title = I18n.T(titleValue);
            }
            else
            {
                title = type.Name;
            }

            if (RequestPermissionDescriptions.TryGetValue(type.Name, out var descriptionValue))
            {
                subTitle = I18n.T(descriptionValue);
            }
            else
            {
                subTitle = type.Name;
            }

            showSnackbar = true;
            InvokeAsync(StateHasChanged);
        }

        private void OnCloseSnackbar()
        {
            showSnackbar = false;
            InvokeAsync(StateHasChanged);
        }
    }
}