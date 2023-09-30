using SwashbucklerDiary.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class RelatedOSPPage : ImportantComponentBase
    {
        private List<OpenSourceProject> OSPs = new();

        protected override async Task OnParametersSetAsync()
        {
            await ReadJson();
            await base.OnParametersSetAsync();
        }

        private async Task OpenBrowser(string? url)
        {
            await PlatformService.OpenBrowser(url);
        }

        private async Task ReadJson()
        {
            OSPs = await PlatformService.ReadJsonFileAsync<List<OpenSourceProject>>("wwwroot/json/open-source-project/open-source-project.json");
        }
    }
}
