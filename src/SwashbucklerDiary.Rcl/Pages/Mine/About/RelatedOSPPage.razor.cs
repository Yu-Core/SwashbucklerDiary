using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class RelatedOSPPage : ImportantComponentBase
    {
        private List<OpenSourceProject> openSourceProjects = [];

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await ReadJson();
        }

        private async Task OpenBrowser(string? url)
        {
            await PlatformIntegration.OpenBrowser(url);
        }

        private async Task ReadJson()
        {
            openSourceProjects = await StaticWebAssets.ReadJsonAsync<List<OpenSourceProject>>("json/open-source-project/open-source-project.json");
        }
    }
}
