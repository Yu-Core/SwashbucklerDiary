using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class SearchAppFunctionPage : ImportantComponentBase
    {
        private string? search;

        private bool showPrivacyModeSearch;

        private string? privacyModeSearchKey;

        private List<AppFunction> allAppFunctions = [];

        private List<AppFunction> _appFunctions = [];

        [Inject]
        protected MasaBlazorHelper MasaBlazorHelper { get; set; } = default!;

        [Parameter]
        [SupplyParameterFromQuery]
        public string? Query { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LoadQuery();
            MasaBlazorHelper.BreakpointChanged += InvokeStateHasChanged;
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

            MasaBlazorHelper.BreakpointChanged -= InvokeStateHasChanged;
        }

        protected override async Task OnResume()
        {
            UpdateAppFunctions();
            await base.OnResume();
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            showPrivacyModeSearch = SettingService.Get<bool>(Setting.HidePrivacyModeEntrance);
            privacyModeSearchKey = SettingService.Get<string>(Setting.PrivacyModeFunctionSearchKey, I18n.T("PrivacyMode.Name"));
        }

        private float ItemHeight => MasaBlazorHelper.Breakpoint.Xs ? 68f : 84f;

        private bool ShowPrivacyModeItem => showPrivacyModeSearch && !string.IsNullOrWhiteSpace(search) && privacyModeSearchKey == search;

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

            if (!string.IsNullOrWhiteSpace(search))
            {
                Expression<Func<AppFunction, bool>> expSearch
                    = it => I18n.T(it.Name ?? string.Empty).Contains(search, StringComparison.CurrentCultureIgnoreCase)
                    || I18n.T(it.Path ?? string.Empty).Contains(search, StringComparison.CurrentCultureIgnoreCase);
                exp = exp.And(expSearch);
            }

            if (exp == null)
            {
                return it => false;
            }

            return exp;
        }

        private void InvokeStateHasChanged(object? sender, MyBreakpointChangedEventArgs e)
        {
            if (!e.XsChanged)
            {
                return;
            }

            InvokeAsync(StateHasChanged);
        }

        private void ToPrivacyMode()
        {
            SettingService.SetTemp<bool>(TempSetting.AllowEnterPrivacyMode, true);
            To("privacyMode");
        }
    }
}
