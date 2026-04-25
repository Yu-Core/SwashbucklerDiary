using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Layout;

namespace SwashbucklerDiary.Rcl.Web.Layout
{
    public class UpdateDialog : UpdateDialogBase
    {
        private const string githubUrl = "https://github.com/Yu-Core/SwashbucklerDiary";

        [Inject]
        private IPlatformIntegration PlatformIntegration { get; set; } = default!;

        protected override async Task ToUpdate()
        {
            await PlatformIntegration.OpenBrowser(githubUrl);
        }
    }
}
