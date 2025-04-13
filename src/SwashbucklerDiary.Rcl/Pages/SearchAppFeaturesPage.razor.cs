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

        private List<AppFeature> allAppFeatures = [];

        private List<AppFeature> _appFeatures = [];

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
            privacyModeSearchKey = SettingService.Get(s => s.PrivacyModeFunctionSearchKey, I18n.T("Privacy mode"));
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
            var appFeatures = await StaticWebAssets.ReadJsonAsync<List<AppFeature>>("json/app-features/app-features.json");
            allAppFeatures = appFeatures;
            UpdateAppFeatures(appFeatures);
        }

        private void UpdateAppFeatures(List<AppFeature> appFeatures)
        {
            Expression<Func<AppFeature, bool>> exp = GetExpression();
            _appFeatures = appFeatures.Where(exp.Compile()).ToList();
        }

        private void UpdateAppFeatures()
            => UpdateAppFeatures(allAppFeatures);

        private Expression<Func<AppFeature, bool>> GetExpression()
        {
            Expression<Func<AppFeature, bool>>? exp = null;

            if (!string.IsNullOrWhiteSpace(search))
            {
                Expression<Func<AppFeature, bool>> expSearch
                    = it => I18n.T(it.Name ?? string.Empty).Contains(search, StringComparison.CurrentCultureIgnoreCase)
                    || I18n.T(it.Path ?? string.Empty).Contains(search, StringComparison.CurrentCultureIgnoreCase);
                exp = exp.And(expSearch);

                Expression<Func<AppFeature, bool>> expPlatform
                    = it => FilterPlatform(it);
                exp = exp.And(expPlatform);
                Expression<Func<AppFeature, bool>> expBreakpoint
                    = it => it.HideBreakpoints == null
                    || !it.HideBreakpoints.Contains(MasaBlazorHelper.Breakpoint.Name.ToString());
                exp = exp.And(expBreakpoint);
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

        static bool FilterPlatform(AppFeature appFeature)
        {
            if (appFeature.HidePlatforms is not null)
            {
                foreach (var item in appFeature.HidePlatforms)
                {
                    var platform = item.Platform;
                    if (string.IsNullOrEmpty(platform)) continue;

                    var version = item.Version;
                    if (version is null)
                    {
                        if (OperatingSystem.IsOSPlatform(platform))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (OperatingSystem.IsOSPlatformVersionAtLeast(platform, version.Major, version.Minor, version.Build, version.Revision))
                        {
                            return false;
                        }
                    }
                }
            }

            if (appFeature.DisplayPlatforms is not null)
            {
                foreach (var item in appFeature.DisplayPlatforms)
                {
                    var platform = item.Platform;
                    if (string.IsNullOrEmpty(platform)) continue;

                    var version = item.Version;
                    if (version is null)
                    {
                        if (!OperatingSystem.IsOSPlatform(platform))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (!OperatingSystem.IsOSPlatformVersionAtLeast(platform, version.Major, version.Minor, version.Build, version.Revision))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        string GetAppFeaturePath(AppFeature appFeature)
        {
            if(appFeature.Path is null)
            {
                return string.Empty;
            }

            var splits = appFeature.Path.Split(" / ").Select(it=> I18n.T(it));
            return string.Join(" / ", splits);
        }
    }
}
