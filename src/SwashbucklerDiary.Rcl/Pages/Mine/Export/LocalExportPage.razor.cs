using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class LocalExportPage : ImportantComponentBase
    {
        private bool showExport;

        private bool showImport;

        private bool showConfirmImport;

        private string? importFilePath;

        private ImportKind importKind;

        private List<DiaryModel> diaries = [];

        private List<DynamicListItem> importTypes = [];

        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;

        [Inject]
        private IDiaryFileManager DiaryFileManager { get; set; } = default!;

        [Inject]
        private ILogger<LocalExportPage> Logger { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LoadView();
        }

        private void LoadView()
        {
            importTypes = new()
            {
                new(this,"Md", "markdown", ImportMd),
                new(this,"Json", "file_json", ImportJson),
            };
        }

        private async Task Export()
        {
            var flag = await PlatformIntegration.TryStorageWritePermission();
            if (!flag)
            {
                await AlertService.InfoAsync(I18n.T("Please grant permission for storage writing"));
                return;
            }

            AlertService.StartLoading();
            try
            {
                diaries = await DiaryService.QueryDiariesAsync();
            }
            finally
            {
                AlertService.StopLoading();
            }

            if (diaries.Count == 0)
            {
                await AlertService.InfoAsync(I18n.T("No diary"));
                return;
            }

            showExport = true;
        }

        private Task ImportJson() => Import(ImportKind.Json);

        private Task ImportMd() => Import(ImportKind.Md);

        private async Task Import(ImportKind kind)
        {
            importKind = kind;
            importFilePath = await PlatformIntegration.PickZipFileAsync();
            if (string.IsNullOrEmpty(importFilePath))
            {
                return;
            }

            showConfirmImport = true;
        }

        private async Task ConfirmImport()
        {
            showConfirmImport = false;
            if (string.IsNullOrEmpty(importFilePath))
            {
                await AlertService.ErrorAsync(I18n.T("Import failed"));
                return;
            }

            AlertService.StartLoading();
            try
            {
                bool isSuccess = false;
                if (importKind == ImportKind.Json)
                {
                    isSuccess = await DiaryFileManager.ImportJsonAsync(importFilePath);
                }
                else if (importKind == ImportKind.Md)
                {
                    isSuccess = await DiaryFileManager.ImportMdAsync(importFilePath);
                }

                if (!isSuccess)
                {
                    await AlertService.ErrorAsync(I18n.T("Import failed"));
                }
                else
                {
                    await AlertService.SuccessAsync(I18n.T("Import successfully"));
                }

            }
            catch (Exception e)
            {
                Logger.LogError(e, "Export Import Fail");
                await AlertService.ErrorAsync(I18n.T("Import failed"));
            }
            finally
            {
                AlertService.StopLoading();
            }
        }
    }
}
