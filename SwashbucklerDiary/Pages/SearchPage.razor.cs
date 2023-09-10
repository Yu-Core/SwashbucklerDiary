using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Extend;
using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Pages
{
    public partial class SearchPage : DiariesPageComponentBase
    {
        private bool ShowFilter;
        private SearchForm SearchForm = new()
        {
            ShowSearch = true
        };

        [Parameter]
        [SupplyParameterFromQuery]
        public string? Query { get; set; }

        protected override async Task OnInitializedAsync()
        {
            LoadCache();
            LoadQuery();
            NavigateService.BeforeNavigate += SetCache;
            await base.OnInitializedAsync();
        }

        protected override async Task UpdateDiariesAsync()
        {
            Expression<Func<DiaryModel, bool>> func = Func();
            var diaries = await DiaryService.QueryAsync(func);
            Diaries = diaries.OrderByDescending(it => it.CreateTime).ToList();
        }

        protected override void OnDispose()
        {
            NavigateService.BeforeNavigate -= SetCache;
            base.OnDispose();
        }

        private bool ShowSearch
        {
            get => SearchForm.ShowSearch;
            set => SearchForm.ShowSearch = value;
        }
        private string? Search
        {
            get => SearchForm.Search;
            set => SearchForm.Search = value;
        }
        private DateOnly DateOnlyMin => SearchForm.DateFilter.GetDateMinValue();
        private DateOnly DateOnlyMax => SearchForm.DateFilter.GetDateMaxValue();
        private bool IsSearchFiltered => !string.IsNullOrWhiteSpace(Search);
        private bool IsDateFiltered => DateOnlyMin != DateOnly.MinValue || DateOnlyMax != DateOnly.MaxValue;

        private void LoadCache()
        {
            SearchForm = (SearchForm?)NavigateService.GetCurrentCache(nameof(SearchForm)) ?? new();
        }

        private void LoadQuery()
        {
            if (!string.IsNullOrEmpty(Query))
            {
                SearchForm.Search = Query;
            }
        }

        private Task SetCache()
        {
            NavigateService.SetCurrentCache(nameof(SearchForm), SearchForm);
            return Task.CompletedTask;
        }

        private Expression<Func<DiaryModel, bool>> Func()
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
