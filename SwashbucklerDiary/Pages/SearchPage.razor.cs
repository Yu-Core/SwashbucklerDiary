using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Extensions;
using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Pages
{
    public partial class SearchPage : DiariesPageComponentBase
    {
        private bool ShowFilter;

        private bool ShowSearch = true;

        private string? Search;

        private DateFilterForm DateFilter = new();

        [Parameter]
        [SupplyParameterFromQuery]
        public string? Query { get; set; }

        protected override async Task OnInitializedAsync()
        {
            LoadQuery();
            await base.OnInitializedAsync();
        }

        protected override async Task UpdateDiariesAsync()
        {
            Expression<Func<DiaryModel, bool>> exp = GetExpression();
            var diaries = await DiaryService.QueryAsync(exp);
            Diaries = diaries.OrderByDescending(it => it.CreateTime).ToList();
        }

        private DateOnly DateOnlyMin => DateFilter.GetDateMinValue();

        private DateOnly DateOnlyMax => DateFilter.GetDateMaxValue();

        private bool IsSearchFiltered => !string.IsNullOrWhiteSpace(Search);

        private bool IsDateFiltered => DateOnlyMin != DateOnly.MinValue || DateOnlyMax != DateOnly.MaxValue;

        private void LoadQuery()
        {
            if (!string.IsNullOrEmpty(Query))
            {
                Search = Query;
            }
        }

        private Expression<Func<DiaryModel, bool>> GetExpression()
        {
            Expression<Func<DiaryModel, bool>>? exp = null;
            Expression<Func<DiaryModel, bool>> expPrivate;
            Expression<Func<DiaryModel, bool>> expSearch;
            Expression<Func<DiaryModel, bool>> expDate;

            expPrivate = it => !it.Private;
            expSearch = it => (it.Title ?? string.Empty).ToLower().Contains((Search ?? string.Empty).ToLower()) ||
                (it.Content ?? string.Empty).ToLower().Contains((Search ?? string.Empty).ToLower());


            DateTime DateTimeMin = DateOnlyMin.ToDateTime(default);
            DateTime DateTimeMax = DateOnlyMax.ToDateTime(TimeOnly.MaxValue);
            expDate = it => it.CreateTime >= DateTimeMin && it.CreateTime <= DateTimeMax;

            if (IsDateFiltered)
            {
                exp = exp.And(expDate);
            }

            if (IsSearchFiltered)
            {
                exp = exp.And(expSearch);
            }

            if (exp == null)
            {
                return it => false;
            }
            else
            {
                return exp.And(expPrivate);
            }
        }
    }
}
