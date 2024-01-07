using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class SearchPage : DiariesPageComponentBase
    {
        private bool showFilter;

        private bool showSearch = true;

        private string? search;

        private DateFilterForm dateFilter = new();

        [Parameter]
        [SupplyParameterFromQuery]
        public string? Query { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LoadQuery();
        }

        protected override async Task UpdateDiariesAsync()
        {
            Expression<Func<DiaryModel, bool>> exp = GetExpression();
            var diaries = await DiaryService.QueryAsync(exp);
            Diaries = diaries.OrderByDescending(it => it.CreateTime).ToList();
        }

        private DateOnly DateOnlyMin => dateFilter.GetDateMinValue();

        private DateOnly DateOnlyMax => dateFilter.GetDateMaxValue();

        private bool IsSearchFiltered => !string.IsNullOrWhiteSpace(search);

        private bool IsDateFiltered => DateOnlyMin != DateOnly.MinValue || DateOnlyMax != DateOnly.MaxValue;

        private void LoadQuery()
        {
            if (!string.IsNullOrEmpty(Query))
            {
                search = Query;
            }
        }

        private Expression<Func<DiaryModel, bool>> GetExpression()
        {
            Expression<Func<DiaryModel, bool>>? exp = null;
            Expression<Func<DiaryModel, bool>> expPrivate;
            Expression<Func<DiaryModel, bool>> expSearch;
            Expression<Func<DiaryModel, bool>> expDate;

            expPrivate = it => !it.Private;
            expSearch = it => (it.Title ?? string.Empty).ToLower().Contains((search ?? string.Empty).ToLower()) ||
                (it.Content ?? string.Empty).ToLower().Contains((search ?? string.Empty).ToLower());

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
