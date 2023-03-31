using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;

namespace SwashbucklerDiary.Pages
{
    public partial class SearchPage : DiariesPageComponentBase
    {
        [Parameter]
        [SupplyParameterFromQuery]
        public string? Search { get; set; }

        protected override Task OnInitializedAsync()
        {
            return base.OnInitializedAsync();
        }

        protected override async Task UpdateDiaries()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Diaries = await DiaryService.QueryAsync(it => !it.Private &&
                    (it.Title!.Contains(Search) || it.Content!.Contains(Search)));
            }
            else
            {
                Diaries = new();
            }
        }

        private async Task TextChanged(string value)
        {
            Search = value;
            await UpdateDiaries();
            var url = Navigation.GetUriWithQueryParameter("Search", value);
            Navigation.NavigateTo(url);
        }
    }
}
