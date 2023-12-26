using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class LocalExport : ImportantComponentBase
    {
        private bool showExport;

        private bool showImport;

        private string? importFilePath;

        private List<DiaryModel> diaries = [];

        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;

        [Inject]
        private IDiaryFileManager DiaryFileManager { get; set; } = default!;

        [Inject]
        private ILogger<LocalExport> Logger { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            diaries = await DiaryService.QueryAsync();
            await base.OnInitializedAsync();
        }

        private async Task Export()
        {
            var flag = await PlatformIntegration.TryStorageWritePermission();
            if (!flag)
            {
                return;
            }

            if (diaries.Count == 0)
            {
                await AlertService.Info(I18n.T("Diary.NoDiary"));
                return;
            }

            showExport = true;
        }

        private async Task Import()
        {
            var flag = await PlatformIntegration.TryStorageWritePermission();
            if (!flag)
            {
                return;
            }

            importFilePath = string.Empty;
            importFilePath = await PlatformIntegration.PickZipFileAsync();
            if (string.IsNullOrEmpty(importFilePath))
            {
                return;
            }
            showImport = true;
        }

        private async Task ConfirmImport()
        {
            showImport = false;
            if (string.IsNullOrEmpty(importFilePath))
            {
                await AlertService.Error(I18n.T("Export.Import.Fail"));
                return;
            }

            try
            {
                bool isSuccess = await DiaryFileManager.ImportJsonAsync(importFilePath);
                if (!isSuccess)
                {
                    await AlertService.Error(I18n.T("Export.Import.Fail"));
                    return;
                }

                await AlertService.Success(I18n.T("Export.Import.Success"));
            }
            catch (Exception e)
            {
                Logger.LogError(e, "Export Import Fail");
                await AlertService.Error(I18n.T("Export.Import.Fail"));
            }
        }
    }
}
