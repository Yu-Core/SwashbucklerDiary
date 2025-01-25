using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class AboutPage : ImportantComponentBase
    {
        private readonly string recordNumber = "辽ICP备2023009191号-2A";

        private bool showSourceCode;

        private bool showUpdate;

        private List<DynamicListItem> codeSourceListItems = [];

        private List<List<DynamicListItem>> viewLists = [];

        [Inject]
        private IAccessExternal AccessExternal { get; set; } = default!;

        [Inject]
        private ILogger<AboutPage> Logger { get; set; } = default!;

        [Inject]
        private IVersionUpdataManager VersionUpdataManager { get; set; } = default!;

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

        private string AppVersion => PlatformIntegration.AppVersionString;

        private void LoadView()
        {
            viewLists =
            [
                [
                    new(this, "About.SourceCode.Name","mdi-book-open-page-variant-outline",() => showSourceCode = true),
                    new(this, "About.Agreement.Name","mdi-file-document-multiple-outline",() => To("user-agreement")),
                    new(this, "About.Privacy.Name","mdi-lock-outline",() => To("privacy-policy")),
                    new(this, "About.Check for updates.Name","mdi-update",CheckForUpdates),
                ],
                [
                    new(this, "About.Related.Name","mdi-xml",() => To("relatedOSP")),
                    new(this, "About.LogFile.Name","mdi-file-document-edit-outline",() => To("log")),
                    new(this, "About.Evaluation.Name","mdi-star-outline",OpenAppDetails),
                    new(this, "About.Sponsor.Name","mdi-hand-heart-outline",()=>To("sponsor")),
                ]
            ];
        }

        private async Task LoadViewAsync()
        {
            var codeSources = await StaticWebAssets.ReadJsonAsync<List<CodeSource>>("json/code-source/code-source.json");
            codeSourceListItems = codeSources.Select(it => new DynamicListItem(this, it.Name ?? string.Empty, it.Icon ?? string.Empty, () => ViewSourceCode(it.Url))).ToList();
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
                await PopupServiceHelper.Error(I18n.T("About.OpenAppStoreFail"));
            }
        }

        private async Task CheckForUpdates()
        {
            try
            {
                bool hasNewVersion = await VersionUpdataManager.CheckForUpdates();
                if (!hasNewVersion)
                {
                    await PopupServiceHelper.Info(I18n.T("VersionUpdate.No updates"));
                }
                else
                {
                    showUpdate = true;
                }
            }
            catch (Exception e)
            {
                await PopupServiceHelper.Error(I18n.T("VersionUpdate.Check failed"));
                Logger.LogError(e, "VersionUpdate check failed");
            }
        }

        private async Task CopyRecordNumber()
        {
            await PlatformIntegration.SetClipboardAsync(recordNumber);
            await PopupServiceHelper.Success(I18n.T("Share.CopySuccess"));
        }
    }
}
