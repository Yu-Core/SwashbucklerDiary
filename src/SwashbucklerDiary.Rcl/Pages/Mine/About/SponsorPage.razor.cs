using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class SponsorPage : ImportantComponentBase
    {
        private string? title;
        private string? src;
        private bool show;
        private List<DynamicListItem> sponsorTypeListItems = [];

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await LoadViewAsync();
        }

        private void OpenSponsorDialog(string? src)
        {
            this.src = src;
            show = true;
        }

        private async Task LoadViewAsync()
        {
            var sponsorTypes = await StaticWebAssets.ReadJsonAsync<List<CodeSource>>("json/sponsor-type/sponsor-type.json");
            List<DynamicListItem> listItems = [];
            foreach (var item in sponsorTypes)
            {
                DynamicListItem sponsorTypeListItem = new(this, item.Name!, item.Icon!, () => OpenSponsorDialog(item.Url));
                listItems.Add(sponsorTypeListItem);
            }
            sponsorTypeListItems = listItems;
        }
    }
}
