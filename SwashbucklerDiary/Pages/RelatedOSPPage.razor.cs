using SwashbucklerDiary.Components;
using System.Text.Json;

namespace SwashbucklerDiary.Pages
{
    public partial class RelatedOSPPage : PageComponentBase
    {
        private List<OSP> OSPs = new();
        private class OSP
        {
            public string Name { get; set; }
            public string License { get; set; }
            public string Url { get; set; }
        }

        protected override async Task OnInitializedAsync()
        {
            await ReadJson();
            await base.OnInitializedAsync();
        }

        private async Task OpenBrowser(string url)
        {
            await SystemService.OpenBrowser(url);
        }

        private async Task ReadJson()
        {
            using Stream streamCultures = await FileSystem.OpenAppPackageFileAsync("wwwroot/osp/osp.json");
            using StreamReader readerCultures = new(streamCultures);
            string contents = readerCultures.ReadToEnd();
            OSPs = JsonSerializer.Deserialize<List<OSP>>(contents) ?? throw new Exception("Failed to read json file data!");
        }
    }
}
