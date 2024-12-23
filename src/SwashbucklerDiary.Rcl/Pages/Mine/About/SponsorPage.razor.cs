using Masa.Blazor;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Models;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class SponsorPage : ImportantComponentBase
    {
        private StringNumber tab = 0;

        private string? src;

        private bool showPreviewImage;

        private List<DynamicListItem> sponsorTypeListItems = [];

        private List<string> sponsors = [];

        private readonly List<TabListItem> tabListItems =
        [
            new("About.Sponsor.Name","sponsor"),
            new("SponsorList.Name","sponsorList"),
        ];

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await LoadViewAsync();
        }

        private void OpenSponsorDialog(string? src)
        {
            this.src = src;
            showPreviewImage = true;
        }

        private async Task LoadViewAsync()
        {
            var sponsorTypes = await StaticWebAssets.ReadJsonAsync<List<CodeSource>>("json/sponsor/sponsor-type.json");
            List<DynamicListItem> listItems = [];
            foreach (var item in sponsorTypes)
            {
                DynamicListItem sponsorTypeListItem = new(this, item.Name!, item.Icon!, () => OpenSponsorDialog(item.Url));
                listItems.Add(sponsorTypeListItem);
            }
            sponsorTypeListItems = listItems;

            sponsors = await StaticWebAssets.ReadJsonAsync<List<string>>("json/sponsor/sponsor-list.json");
        }
    }
}
