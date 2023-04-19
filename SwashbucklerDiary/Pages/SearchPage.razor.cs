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
        [Parameter]
        [SupplyParameterFromQuery]
        public DateTime MinDateTime
        {
            get => MinDate.ToDateTime(default);
            set
            {
                if (value == DateTime.MinValue)
                {
                    return;
                }

                MinDate = DateOnly.FromDateTime(value);
            }
        }
        [Parameter]
        [SupplyParameterFromQuery]
        public DateTime MaxDateTime
        {
            get => MaxDate.ToDateTime(TimeOnly.MaxValue);
            set
            {
                if (value == DateTime.MinValue)
                {
                    return;
                }

                MaxDate = DateOnly.FromDateTime(value);
            }
        }

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
                return Navigation.GetUriWithQueryParameters(
                    new Dictionary<string, object?>
                    {
                        ["Search"] = Search,
                        ["MinDateTime"] = MinDateTime,
                        ["MaxDateTime"] = MaxDateTime,
                    });
            };
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
            expDate = it => it.CreateTime >= MinDateTime && it.CreateTime <= MaxDateTime;

            if(IsDateFiltered)
            {
                exp = exp.And(expDate);
            }

            if (IsSearchFiltered)
            {
                exp = exp.And(expSearch);
            }

            if(exp == null)
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
