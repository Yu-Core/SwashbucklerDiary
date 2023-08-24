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
            LoadCache();
            await LoadSettings();
            await SetAppFunctions();
            UpdateAppFunctions();
            NavigateService.BeforeNavigate += SetCache;
            await base.OnInitializedAsync();
        }

        protected override void OnDispose()
        {
            NavigateService.BeforeNavigate -= SetCache;
            base.OnDispose();
        }

        protected override void NavigateToBack()
        {
            if (ShowSearch)
            {
                ShowSearch = false;
                return;
            }
            base.NavigateToBack();
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

        private async Task SetAppFunctions()
        {
            var appFunctions = await PlatformService.ReadJsonFileAsync<List<AppFunction>>("wwwroot/json/app-functions/app-functions.json");
            AppFunctions = AllAppFunctions = appFunctions;
        }

        private async Task LoadSettings()
        {
            Privacy = await SettingsService.Get(SettingType.PrivacyMode);
        }


        private void LoadCache()
        {
            SearchForm = (SearchForm?)NavigateService.GetCurrentCache(nameof(SearchForm)) ?? new();
        }

        private Task SetCache()
        {
            NavigateService.SetCurrentCache(nameof(SearchForm), SearchForm);
            return Task.CompletedTask;
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

            expSearch = it => I18n.T(it.Name ?? string.Empty).ToLower().Contains((Search ?? string.Empty).ToLower())
                || I18n.T(it.Path ?? string.Empty).ToLower().Contains((Search ?? string.Empty).ToLower());
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
