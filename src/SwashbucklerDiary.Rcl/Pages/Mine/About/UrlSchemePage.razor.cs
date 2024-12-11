using Masa.Blazor;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Models;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class UrlSchemePage : ImportantComponentBase
    {
        private StringNumber? scheme;

        private List<UrlScheme> urlSchemes = [];

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await ReadJson();
        }

        private async Task ReadJson()
        {
            urlSchemes = await StaticWebAssets.ReadJsonAsync<List<UrlScheme>>("json/url-scheme/url-scheme.json");
        }

        private async Task CopyAsync(string path)
        {
            var text = $"{scheme}://{path}";
            await PlatformIntegration.SetClipboard(text);
            await PopupServiceHelper.Success(I18n.T("Share.CopySuccess"));
        }
    }
}