using SwashbucklerDiary.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class SponsorPage : ImportantComponentBase
    {
        private List<DynamicListItem> SponsorTypeListItems = new();

        protected override async Task OnInitializedAsync()
        {
            await LoadViewAsync();
            await base.OnInitializedAsync();
        }

        private async Task ToSponsor(string? url)
        {
            await PlatformService.OpenBrowser(url);
        }

        private async Task LoadViewAsync()
        {
            var sponsorTypes = await PlatformService.ReadJsonFileAsync<List<CodeSource>>("wwwroot/json/sponsor-type/sponsor-type.json");
            foreach (var item in sponsorTypes)
            {
                DynamicListItem sponsorTypeListItem = new(this, item.Name, item.Icon, () => ToSponsor(I18n.T(item.Url)));
                SponsorTypeListItems.Add(sponsorTypeListItem);
            }
        }
    }
}
