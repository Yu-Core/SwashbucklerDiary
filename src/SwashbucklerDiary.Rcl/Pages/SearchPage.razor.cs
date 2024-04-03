using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class SearchPage : DiariesPageComponentBase
    {
        private bool showFilter;

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

        private bool IsDateFiltered => DateOnlyMin != default || DateOnlyMax != default;

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

            if (DateOnlyMin != default)
            {
                DateTime DateTimeMin = DateOnlyMin.ToDateTime(default);
                Expression<Func<DiaryModel, bool>> expMinDate = it => it.CreateTime >= DateTimeMin;
                exp = exp.And(expMinDate);
            }

            if (DateOnlyMax != default)
            {
                DateTime DateTimeMax = DateOnlyMax.ToDateTime(TimeOnly.MaxValue);
                Expression<Func<DiaryModel, bool>> expMaxDate = it => it.CreateTime <= DateTimeMax;
                exp = exp.And(expMaxDate);
            }

            if (IsSearchFiltered)
            {
                Expression<Func<DiaryModel, bool>> expSearch
                    = it => (it.Title ?? string.Empty).Contains(search ?? string.Empty, StringComparison.CurrentCultureIgnoreCase)
                    || (it.Content ?? string.Empty).Contains(search ?? string.Empty, StringComparison.CurrentCultureIgnoreCase);
                exp = exp.And(expSearch);
            }

            if (exp == null)
            {
                return it => false;
            }
            else
            {
                Expression<Func<DiaryModel, bool>> expPrivate = it => !it.Private;
                return exp.And(expPrivate);
            }
        }
    }
}
