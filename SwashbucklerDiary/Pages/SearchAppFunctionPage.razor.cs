using SwashbucklerDiary.Components;
using SwashbucklerDiary.Extend;
using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Pages
{
    public partial class SearchAppFunctionPage : PageComponentBase
    {
        private bool Privacy;
        private List<AppFunction> AllAppFunctions = new();
        private List<AppFunction> AppFunctions = new();
        private SearchForm SearchForm = new()
        {
            ShowSearch = true
        };

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await SetAppFunctions();
            HandleCurrentCache();
            UpdateAppFunctions();
            await base.OnInitializedAsync();
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
        private bool IsSearchFiltered => !string.IsNullOrWhiteSpace(Search);

        protected override void NavigateToBack()
        {
            if (ShowSearch)
            {
                ShowSearch = false;
                return;
            }
            base.NavigateToBack();
        }

        private async Task SetAppFunctions()
        {
            var appFunctions = await PlatformService.ReadJsonFileAsync<List<AppFunction>>("wwwroot/json/app-functions/app-functions.json");
            AppFunctions = AllAppFunctions = appFunctions;
        }

        private void HandleCurrentCache()
        {
            SearchForm = (SearchForm?)NavigateService.GetCurrentCache() ?? new();
            NavigateService.SetCurrentCache(() => SearchForm);
        }

        private async Task LoadSettings()
        {
            Privacy = await SettingsService.Get(SettingType.PrivacyMode);
        }

        private void UpdateAppFunctions()
        {
            Expression<Func<AppFunction, bool>> func = Func();
            AppFunctions = AllAppFunctions.Where(func.Compile()).ToList();
        }

        private Expression<Func<AppFunction, bool>> Func()
        {
            Expression<Func<AppFunction, bool>>? exp = null;
            Expression<Func<AppFunction, bool>> expSearch;
            Expression<Func<AppFunction, bool>> expPrivacy;

            expSearch = it => I18n.T(it.Name ?? string.Empty).ToLower().Contains((Search ?? string.Empty).ToLower());
            expPrivacy = it => !it.ConditionalDisplay || it.Privacy == Privacy;

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
                exp = exp.And(expPrivacy);
            }

            return exp;
        }
    }
}
