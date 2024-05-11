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
            title = I18n.T($"PermissionDescription.{type.Name}.Name");
            subTitle = I18n.T($"PermissionDescription.{type.Name}.Description");
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