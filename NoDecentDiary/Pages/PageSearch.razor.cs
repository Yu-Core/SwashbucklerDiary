using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using NoDecentDiary.Interface;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Pages
{
    public partial class PageSearch : INavigateToBack
    {
        [Inject]
        private IDiaryService? DiaryService { get; set; }
        [Inject]
        public NavigationManager? Navigation { get; set; }
        [Parameter]
        [SupplyParameterFromQuery]
        public string? Href { get; set; }
        [Parameter]
        [SupplyParameterFromQuery]
        public string? Search { get; set; }
        private List<DiaryModel> Diaries = new();
        protected override async Task OnInitializedAsync()
        {
            await UpdateDiaries();
        }
        private async Task UpdateDiaries()
        {
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Diaries = await DiaryService!.QueryAsync(it =>
                    it.Title!.Contains(Search) ||
                    it.Content!.Contains(Search));
            }
            else
            {
                Diaries = new();
            }
        }

        private async Task HandOnTextChanged(string value)
        {
            Search = value;
            await UpdateDiaries();
            var url = Navigation!.GetUriWithQueryParameter("Search", value);
            Navigation!.NavigateTo(url);
        }
        private void HandOnBack()
        {
            NavigateToBack();
        }

        public void NavigateToBack()
        {
            this.DefaultNavigateToBack();
        }
    }
}
