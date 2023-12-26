using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class SponsorPage : ImportantComponentBase
    {
        private readonly List<DynamicListItem> sponsorTypeListItems = [];

        protected override async Task OnInitializedAsync()
        {
            await LoadViewAsync();
            await base.OnInitializedAsync();
        }

        private async Task ToSponsor(string? url)
        {
            await PlatformIntegration.OpenBrowser(url);
        }

        private async Task LoadViewAsync()
        {
            var sponsorTypes = await StaticWebAssets.ReadJsonAsync<List<CodeSource>>("json/sponsor-type/sponsor-type.json");
            foreach (var item in sponsorTypes)
            {
                DynamicListItem sponsorTypeListItem = new(this, item.Name, item.Icon, () => ToSponsor(I18n.T(item.Url)));
                sponsorTypeListItems.Add(sponsorTypeListItem);
            }
        }
    }
}
