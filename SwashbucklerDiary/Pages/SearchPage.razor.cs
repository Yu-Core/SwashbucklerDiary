using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Extend;
using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Pages
{
    public partial class SearchPage : DiariesPageComponentBase
    {
        private bool ShowSearch = true;
        private bool ShowFilter;
        private DateOnly MinDate;
        private DateOnly MaxDate = DateOnly.MaxValue;

        [Parameter]
        [SupplyParameterFromQuery]
        public string? Search { get; set; }

        protected override async Task OnInitializedAsync()
        {
            SetCurrentUrl();
            await base.OnInitializedAsync();
        }

        protected override async Task UpdateDiaries()
        {
            Expression<Func<DiaryModel, bool>> func = Func();
            var diaries = await DiaryService.QueryAsync(func);
            Diaries = diaries.OrderByDescending(it => it.CreateTime).ToList();
        }

        private bool IsSearchFiltered => !string.IsNullOrWhiteSpace(Search);
        private bool IsDateFiltered => MinDate != DateOnly.MinValue || MaxDate != DateOnly.MaxValue;

        private void SetCurrentUrl()
        {
            NavigateService.CurrentUrl += () =>
            {
                return Navigation.GetUriWithQueryParameter("Search", Search);
            };
        }

        private Expression<Func<DiaryModel, bool>> Func()
        {
            Expression<Func<DiaryModel, bool>> expSearch;
            Expression<Func<DiaryModel, bool>> expDate;

            if (!IsSearchFiltered)
            {
                expSearch = it => false;
            }
            else
            {
                expSearch = it => !it.Private &&
                            ((it.Title ?? string.Empty).ToLower().Contains((Search ?? string.Empty).ToLower()) ||
                            (it.Content ?? string.Empty).ToLower().Contains((Search ?? string.Empty).ToLower()));

            }

            DateTime MinDateTime = MinDate.ToDateTime(default);
            DateTime MaxDateTime = MaxDate.ToDateTime(TimeOnly.MaxValue);
            if (MaxDate != DateOnly.MaxValue)
            {
                MaxDateTime = MaxDateTime.AddDays(1);
            }

            expDate = it => it.CreateTime >= MinDateTime && it.CreateTime <= MaxDateTime;
            return expSearch.And(expDate);
        }

    }
}
