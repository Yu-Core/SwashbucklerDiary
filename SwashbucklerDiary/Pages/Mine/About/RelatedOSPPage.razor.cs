using SwashbucklerDiary.Components;
using SwashbucklerDiary.Models;
using System.Text.Json;

namespace SwashbucklerDiary.Pages
{
    public partial class RelatedOSPPage : PageComponentBase
    {
        private List<OpenSourceProject> OSPs = new();

        protected override async Task OnInitializedAsync()
        {
            await ReadJson();
            await base.OnInitializedAsync();
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
