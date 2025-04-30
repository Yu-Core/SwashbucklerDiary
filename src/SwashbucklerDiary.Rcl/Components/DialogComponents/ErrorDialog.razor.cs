using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class ErrorDialog
    {
        [Inject]
        private II18nService I18n { get; set; } = default!;

        [Inject]
        private IAppLifecycle AppLifecycle { get; set; } = default!;

        [Inject]
        private IPlatformIntegration PlatformIntegration { get; set; } = default!;

        [Inject]
        private IAlertService AlertService { get; set; } = default!;

        [CascadingParameter(Name = "Culture")]
        public string? Culture { get; set; }

        [Parameter]
        public Exception? Exception { get; set; }

        private string? Text => Exception is null ? null : $"{Exception.Message}\n{Exception.StackTrace}";

        private async Task CopyDetailedInformationAsync()
        {
            if (Text is not null)
            {
                await PlatformIntegration.SetClipboardAsync(Text);
                await AlertService.Success(I18n.T("Copy successfully"));
            }
        }

        private void Quit()
        {
            AppLifecycle.QuitApp();
        }
    }
}