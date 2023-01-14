using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using NoDecentDiary.IServices;
using NoDecentDiary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NoDecentDiary.Pages
{
    public partial class PageSearch
    {
        [Inject]
        private IDiaryService? DiaryService { get; set; }
        [Inject]
        private NavigationManager? Navigation { get; set; }
        [Parameter]
        [SupplyParameterFromQuery]
        public string? Href { get; set; }
        private string? _search;
        private List<DiaryModel> Diaries = new();

        private async Task HandOnTextChanged(string value)
        {
            _search = value;
            if(!string.IsNullOrWhiteSpace(_search))
            {
                Diaries = await DiaryService!.QueryAsync(it =>
                    it.Title!.Contains(_search)||
                    it.Content!.Contains(_search));
            }
            else
            {
                Diaries = new();
            }
        }
        private void HandOnBack()
        {
            if (Href != null)
            {
                Navigation!.NavigateTo(Href);
            }
            else
            {
                Navigation!.NavigateTo("/");
            }
        }
    }
}
