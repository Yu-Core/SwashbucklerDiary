using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class SearchPage : PageComponentBase
    {
        private List<DiaryModel> Diaries = new();

        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;

        [Parameter]
        [SupplyParameterFromQuery]
        public string? Search { get; set; }

        protected override async Task OnInitializedAsync()
        {
            await UpdateDiaries();
        }

        private async Task UpdateDiaries()
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
