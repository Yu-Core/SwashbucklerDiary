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
        private Expression<Func<DiaryModel, bool>>? _dateExpression = null;
        private Expression<Func<DiaryModel, bool>>? _searchExpression = null;
        private Func<DiaryModel, string, bool> SearchFunc = (it, search) =>
        {
            return !it.Private &&
            ((it.Title ?? string.Empty).Contains(search) ||
            (it.Content ?? string.Empty).Contains(search));
        };
        //private List<DiaryModel> NewDiaries = new();

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
            Func<DiaryModel, bool>? exp = DateExpression.Compile();
            Func<DiaryModel, bool>? exp2 = SearchExpression.Compile();
            //NewDiaries = Diaries.Where(exp).Where(exp2).ToList();
            //Expression<Func<DiaryModel, bool>> expression = DateExpression.And(SearchExpression);
            Diaries = await DiaryService.QueryAsync(it=>exp.Invoke(it) && exp2.Invoke(it));
            await Task.CompletedTask;
        }

        private Expression<Func<DiaryModel, bool>> DateExpression => GetDateExpression();
        private Expression<Func<DiaryModel, bool>> SearchExpression => GetSearchExpression();
        private bool IsFilter => _dateExpression != null;

        private void SetCurrentUrl()
        {
            NavigateService.CurrentUrl += () =>
            {
                return Navigation.GetUriWithQueryParameter("Search", Search);
            };
        }

        private Expression<Func<DiaryModel, bool>> GetDateExpression()
        {
            if (_dateExpression == null)
            {
                Expression<Func<DiaryModel, bool>> exp = it => true;
                return exp;
            }

            return _dateExpression;
        }

        private Expression<Func<DiaryModel, bool>> GetSearchExpression()
        {
            if (_searchExpression == null)
            {
                Expression<Func<DiaryModel, bool>> exp = it => false;
                return exp;
            }

            return _searchExpression;
        }

    }
}
