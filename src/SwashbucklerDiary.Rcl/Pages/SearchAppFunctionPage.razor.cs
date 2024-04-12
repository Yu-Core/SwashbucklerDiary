using Masa.Blazor;
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

        private List<AppFunction> allAppFunctions = [];

        private List<AppFunction> _appFunctions = [];

        [Inject]
        protected MasaBlazor MasaBlazor { get; set; } = default!;

        [Parameter]
        [SupplyParameterFromQuery]
        public string? Query { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LoadQuery();
            MasaBlazor.BreakpointChanged += InvokeStateHasChanged;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                await LoadAppFunctions();
                StateHasChanged();
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            MasaBlazor.BreakpointChanged -= InvokeStateHasChanged;
        }

        protected override async Task OnResume()
        {
            UpdateAppFunctions();
            await base.OnResume();
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            privacy = SettingService.Get<bool>(Setting.PrivacyMode);
        }

        private bool IsSearchFiltered => !string.IsNullOrWhiteSpace(search);

        private float ItemHeight => MasaBlazor.Breakpoint.Xs ? 68f : 84f;

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
            allAppFunctions = appFunctions;
            UpdateAppFunctions(appFunctions);
        }

        private void UpdateAppFunctions(List<AppFunction> appFunctions)
        {
            Expression<Func<AppFunction, bool>> exp = GetExpression();
            _appFunctions = appFunctions.Where(exp.Compile()).ToList();
        }

        private void UpdateAppFunctions()
            => UpdateAppFunctions(allAppFunctions);

        private Expression<Func<AppFunction, bool>> GetExpression()
        {
            Expression<Func<AppFunction, bool>>? exp = null;

            if (IsSearchFiltered)
            {
                Expression<Func<AppFunction, bool>> expSearch
                    = it => I18n.T(it.Name ?? string.Empty).Contains(search ?? string.Empty, StringComparison.CurrentCultureIgnoreCase)
                    || I18n.T(it.Path ?? string.Empty).Contains(search ?? string.Empty, StringComparison.CurrentCultureIgnoreCase);
                exp = exp.And(expSearch);
            }

            if (exp == null)
            {
                return it => false;
            }
            else
            {
                Expression<Func<AppFunction, bool>> expPrivacy = it => !it.ConditionalDisplay || it.Privacy == privacy;
                exp = exp.And(expPrivacy);
            }

            return exp;
        }

        private void InvokeStateHasChanged(object? sender, BreakpointChangedEventArgs e)
        {
            InvokeAsync(StateHasChanged);
        }
    }
}
