using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using System.Net.Http.Json;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class AboutPage : ImportantComponentBase
    {
        private bool showSourceCode;

        private bool showVersionUpdate;

        private List<DynamicListItem> codeSourceListItems = [];

        private List<List<DynamicListItem>> viewLists = [];

        [Inject]
        private IAccessExternal AccessExternal { get; set; } = default!;

        [Inject]
        private ILogger<AboutPage> Logger { get; set; } = default!;

        private class Release
        {
            public string? Tag_Name { get; set; }
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LoadView();
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await LoadViewAsync();
        }

        private string AppVersion => PlatformIntegration.AppVersion;

        private void LoadView()
        {
            viewLists = new()
            {
                new()
                {
                    new(this, "About.SourceCode.Name","mdi-book-open-page-variant-outline",() => showSourceCode = true),
                    new(this, "About.Agreement.Name","mdi-file-document-multiple-outline",() => To("user-agreement")),
                    new(this, "About.Privacy.Name","mdi-lock-outline",() => To("privacy-policy")),
                    new(this, "About.Check for updates.Name","mdi-update",VersionUpdate),
                },
                new()
                {
                    new(this, "About.Related.Name","mdi-xml",() => To("relatedOSP")),
                    new(this, "About.LogFile.Name","mdi-file-document-edit-outline",() => To("log")),
                    new(this, "About.Evaluation.Name","mdi-star-outline",OpenAppDetails),
                    new(this, "About.Sponsor.Name","mdi-hand-heart-outline",()=>To("sponsor")),
                }
            };
        }

        private async Task LoadViewAsync()
        {
            var codeSources = await StaticWebAssets.ReadJsonAsync<List<CodeSource>>("json/code-source/code-source.json");
            foreach (var item in codeSources)
            {
                DynamicListItem codeSourceListItem = new(this, item.Name, item.Icon, () => ViewSourceCode(item.Url));
                codeSourceListItems.Add(codeSourceListItem);
            }
        }

        private async Task ViewSourceCode(string? url)
        {
            await PlatformIntegration.OpenBrowser(url);
            await HandleAchievements(Achievement.SourceCode);
        }

        private async Task OpenAppDetails()
        {
            bool flag = await AccessExternal.OpenAppStoreAppDetails();
            if (!flag)
            {
                await AlertService.Error(I18n.T("About.OpenAppStoreFail"));
            }
        }

        private async Task VersionUpdate()
        {
            try
            {
                using HttpClient httpClient = new();
                httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; Trident/6.0)");
                var release = await httpClient.GetFromJsonAsync<Release>(I18n.T("VersionUpdate.LatestVersionUrl"));
                if (release is null || release.Tag_Name is null)
                {
                    await AlertService.Error(I18n.T("VersionUpdate.Check failed"));
                    return;
                }

                string latestVersion = release.Tag_Name.TrimStart('v').Replace(".0", "");
                string currentVersion = AppVersion.Replace(".0", "");
                if (latestVersion == currentVersion)
                {
                    await AlertService.Info(I18n.T("VersionUpdate.No updates"));
                }
                else
                {
                    showVersionUpdate = true;
                }
            }
            catch (Exception e)
            {
                await AlertService.Error(I18n.T("VersionUpdate.Check failed"));
                Logger.LogError(e, "VersionUpdate check failed");
            }

        }

        private Task ToUpdate()
        {
            showVersionUpdate = false;
            StateHasChanged();
            return OpenAppDetails();
        }
    }
}
