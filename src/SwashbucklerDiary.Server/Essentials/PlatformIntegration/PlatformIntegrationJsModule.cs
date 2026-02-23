using Microsoft.JSInterop;

namespace SwashbucklerDiary.Server.Essentials
{
    public class PlatformIntegrationJSModule : Rcl.Web.Essentials.PlatformIntegrationJSModule
    {
        public PlatformIntegrationJSModule(IJSRuntime js) : base(js, "./js/platformIntegration.js")
        {
        }

        public override ValueTask<string[]?> PickFilesAsync(
            string accept,
            string[] fileExtensions,
            bool multiple = true)
        {
            return InvokeAsync<string[]?>("chooseFiles", accept, fileExtensions, multiple);
        }
    }
}
