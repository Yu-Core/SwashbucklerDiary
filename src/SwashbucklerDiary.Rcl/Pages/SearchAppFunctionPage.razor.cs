using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class SearchAppFunctionPage : ImportantComponentBase
    {
        private bool privacy;

        private string? search;

        private List<AppFunction> AllAppFunctions = [];

        private List<AppFunction> AppFunctions = [];
        
        [Parameter]
        [SupplyParameterFromQuery]
        public string? Query { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LoadQuery();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if(firstRender)
            {
                await Task.WhenAll(
                    UpdateSettings(),
                    LoadAppFunctions());
                StateHasChanged();
            }
        }

        protected override async Task OnResume()
        {
            await UpdateSettings();
            UpdateAppFunctions();
            await base.OnResume();
        }

        private bool IsSearchFiltered => !string.IsNullOrWhiteSpace(search);

        private void LoadQuery()
        {
            if (!string.IsNullOrEmpty(Query))
            {
                search = Query;
            }
        }

        private async Task LoadAppFunctions()
        {
            var appFunctions = await StaticWebAssets.ReadJsonAsync<List<AppFunction>>("json/app-functions/app-functions.json");
            AllAppFunctions = appFunctions;
            UpdateAppFunctions(appFunctions);
        }

        private async Task UpdateSettings()
        {
            privacy = await SettingService.Get<bool>(Setting.PrivacyMode);
        }

        private void UpdateAppFunctions(List<AppFunction> appFunctions)
        {
            Expression<Func<AppFunction, bool>> exp = GetExpression();
            AppFunctions = appFunctions.Where(exp.Compile()).ToList();
        }

        private void UpdateAppFunctions()
            => UpdateAppFunctions(AllAppFunctions);

        private Expression<Func<AppFunction, bool>> GetExpression()
        {
            Expression<Func<AppFunction, bool>>? exp = null;
            Expression<Func<AppFunction, bool>> expSearch;
            Expression<Func<AppFunction, bool>> expPrivacy;

            expSearch = it => I18n.T(it.Name ?? string.Empty).ToLower().Contains((search ?? string.Empty).ToLower())
                || I18n.T(it.Path ?? string.Empty).ToLower().Contains((search ?? string.Empty).ToLower());
            expPrivacy = it => !it.ConditionalDisplay || it.Privacy == privacy;

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
