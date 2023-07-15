using Serilog;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Models;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text.Json;

namespace SwashbucklerDiary.Pages
{
    public partial class AboutPage : PageComponentBase
    {
        private bool ShowSourceCode;
        private bool ShowSponsor;
        private bool ShowVersionUpdate;
        private readonly List<DynamicListItem> CodeSources = new();
        private List<List<DynamicListItem>> ViewLists = new();

        private class Release
        {
            public string? Tag_Name { get; set;}
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadView();
            await base.OnInitializedAsync();
        }

        private async Task LoadView()
        {
            ViewLists = new()
            {
                new()
                {
                    new(this, "About.SourceCode.Name","mdi-book-open-page-variant-outline",() => ShowSourceCode = true),
                    new(this, "About.Agreement.Name","mdi-file-document-multiple-outline",() => To("user-agreement")),
                    new(this, "About.Privacy.Name","mdi-lock-outline",() => To("privacy-policy")),
                    new(this, "About.Check for updates.Name","mdi-update",VersionUpdate),
                },
                new()
                {
                    new(this, "About.Related.Name","mdi-xml",() => To("relatedOSP")),
                    new(this, "About.LogFile.Name","mdi-file-document-edit-outline",() => To("log")),
                    new(this, "About.Evaluation.Name","mdi-star-outline",OpenAppDetails),
                    new(this, "About.Sponsor.Name","mdi-hand-heart-outline",() => ShowSponsor = true),
                }
            };

            var codeSources = await PlatformService.ReadJsonFileAsync<List<CodeSource>>("wwwroot/json/code-source/code-source.json");
            foreach (var item in codeSources)
            {
                DynamicListItem codeSource = new(this, item.Name, item.Icon, () => ViewSourceCode(item.Url));
                CodeSources.Add(codeSource);
            }
        }

        private string AppVersion => PlatformService.GetAppVersion();

        private async Task ViewSourceCode(string? url)
        {
            await PlatformService.OpenBrowser(url);
            await HandleAchievements(AchievementType.SourceCode);
        }

        private async Task OpenAppDetails()
        {
            bool flag = await PlatformService.OpenStoreMyAppDetails();
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
                    ShowVersionUpdate = true;
                }
            }
            catch (Exception e)
            {
                await AlertService.Error(I18n.T("VersionUpdate.Check failed"));
                Log.Error($"{e.Message}\n{e.StackTrace}");
            }
            
        }

        private Task ToUpdate()
        {
            ShowVersionUpdate = false;
            StateHasChanged();
            return OpenAppDetails();
        }
    }
}
