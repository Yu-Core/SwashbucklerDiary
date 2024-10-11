using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class SearchAppFeaturesPage : ImportantComponentBase
    {
        private string? search;

        private bool showPrivacyModeSearch;

        private string? privacyModeSearchKey;

        private List<AppFeatures> allAppFeatures = [];

        private List<AppFeatures> _appFeatures = [];

        [Inject]
        protected MasaBlazorHelper MasaBlazorHelper { get; set; } = default!;

        [Parameter]
        [SupplyParameterFromQuery]
        public string? Query { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LoadQuery();
            MasaBlazorHelper.BreakpointChanged += HandleBreakpointChange;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                await LoadAppFeatures();
                StateHasChanged();
            }
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            MasaBlazorHelper.BreakpointChanged -= HandleBreakpointChange;
        }

        protected override async Task OnResume()
        {
            UpdateAppFeatures();
            await base.OnResume();
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            showPrivacyModeSearch = SettingService.Get(s => s.HidePrivacyModeEntrance);
            privacyModeSearchKey = SettingService.Get(s => s.PrivacyModeFunctionSearchKey, I18n.T("PrivacyMode.Name"));
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

        private async Task LoadAppFeatures()
        {
            var appFeatures = await StaticWebAssets.ReadJsonAsync<List<AppFeatures>>("json/app-features/app-features.json");
            allAppFeatures = appFeatures;
            UpdateAppFeatures(appFeatures);
        }

        private void UpdateAppFeatures(List<AppFeatures> appFeatures)
        {
            Expression<Func<AppFeatures, bool>> exp = GetExpression();
            _appFeatures = appFeatures.Where(exp.Compile()).ToList();
        }

        private void UpdateAppFeatures()
            => UpdateAppFeatures(allAppFeatures);

        private Expression<Func<AppFeatures, bool>> GetExpression()
        {
            Expression<Func<AppFeatures, bool>>? exp = null;

            if (!string.IsNullOrWhiteSpace(search))
            {
                Expression<Func<AppFeatures, bool>> expSearch
                    = it => I18n.T(it.Name ?? string.Empty).Contains(search, StringComparison.CurrentCultureIgnoreCase)
                    || I18n.T(it.Path ?? string.Empty).Contains(search, StringComparison.CurrentCultureIgnoreCase);
                exp = exp.And(expSearch);

                Expression<Func<AppFeatures, bool>> expPlatform
                    = it => it.HidePlatforms == null
                    || !it.HidePlatforms.Contains(PlatformIntegration.CurrentPlatform.ToString());
                exp = exp.And(expPlatform);
            }

            if (exp == null)
            {
                return it => false;
            }

            return exp;
        }

        private void HandleBreakpointChange(object? sender, MyBreakpointChangedEventArgs e)
        {
            if (!e.XsChanged)
            {
                return;
            }

            InvokeAsync(StateHasChanged);
        }

        private void ToPrivacyMode()
        {
            SettingService.SetTemp(s => s.AllowEnterPrivacyMode, true);
            To("privacyMode");
        }
    }
}
