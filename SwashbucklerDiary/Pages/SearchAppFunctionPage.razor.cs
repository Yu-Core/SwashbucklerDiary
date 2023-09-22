using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Extensions;
using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Pages
{
    public partial class SearchAppFunctionPage : ImportantComponentBase
    {
        private bool Privacy;
        private string? Search;
        private List<AppFunction> AllAppFunctions = new();
        private List<AppFunction> AppFunctions = new();
        
        [Parameter]
        [SupplyParameterFromQuery]
        public string? Query { get; set; }

        protected override async Task OnInitializedAsync()
        {
            LoadQuery();
            await LoadSettings();
            await LoadAppFunctions();
            UpdateAppFunctions();
            await base.OnInitializedAsync();
        }

        protected override async void OnResume()
        {
            await LoadSettings();
            UpdateAppFunctions();
            base.OnResume();
        }

        private bool IsSearchFiltered => !string.IsNullOrWhiteSpace(Search);

        private void LoadQuery()
        {
            if (!string.IsNullOrEmpty(Query))
            {
                Search = Query;
            }
        }

        private async Task LoadAppFunctions()
        {
            var appFunctions = await PlatformService.ReadJsonFileAsync<List<AppFunction>>("wwwroot/json/app-functions/app-functions.json");
            AllAppFunctions = appFunctions;
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
