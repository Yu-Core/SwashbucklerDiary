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
            await SystemService.OpenBrowser(url);
        }

        private async Task ReadJson()
        {
            using Stream streamCultures = await FileSystem.OpenAppPackageFileAsync("wwwroot/json/open-source-project/open-source-project.json");
            using StreamReader readerCultures = new(streamCultures);
            string contents = readerCultures.ReadToEnd();
            OSPs = JsonSerializer.Deserialize<List<OpenSourceProject>>(contents) ?? throw new Exception("Failed to read json file data!");
        }
    }
}
