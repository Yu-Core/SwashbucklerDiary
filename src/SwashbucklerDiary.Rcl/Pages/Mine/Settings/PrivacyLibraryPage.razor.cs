using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class PrivacyLibraryPage : DiariesPageComponentBase
    {
        private bool showSearch;

        private bool showFilter;

        private string? search;

        private DateFilterForm dateFilter = new();

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

        private Expression<Func<DiaryModel, bool>> GetExpression()
        {
            Expression<Func<DiaryModel, bool>> expPrivate;
            Expression<Func<DiaryModel, bool>> expSearch;
            Expression<Func<DiaryModel, bool>> expDate;

            expPrivate = it => it.Private;
            expSearch = it => (it.Title ?? string.Empty).ToLower().Contains((search ?? string.Empty).ToLower()) ||
                            (it.Content ?? string.Empty).ToLower().Contains((search ?? string.Empty).ToLower());
            
            DateTime DateTimeMin = DateOnlyMin.ToDateTime(default);
            DateTime DateTimeMax = DateOnlyMax.ToDateTime(TimeOnly.MaxValue);
            expDate = it => it.CreateTime >= DateTimeMin && it.CreateTime <= DateTimeMax;
            return expPrivate.And(expDate).AndIF(IsSearchFiltered, expSearch);
        }
    }
}
