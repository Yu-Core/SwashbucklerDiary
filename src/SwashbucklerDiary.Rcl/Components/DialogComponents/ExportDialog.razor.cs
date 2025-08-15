using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class ExportDialog : DialogComponentBase
    {
        private List<DynamicListItem> items = [];

        [Inject]
        protected IDiaryFileManager DiaryFileManager { get; set; } = default!;

        [Inject]
        private ILogger<ExportDialog> Logger { get; set; } = default!;

        [Inject]
        private IPlatformIntegration PlatformIntegration { get; set; } = default!;

        [Parameter]
        public List<DiaryModel> Value { get; set; } = [];

        protected override void OnInitialized()
        {
            LoadView();
            base.OnInitialized();
        }

        private void LoadView()
        {
            items =
            [
                new(this,"Txt", "description", ExportTxtAsync),
                new(this,"Md", "markdown", ExportMdAsync),
                new(this,"Json", "file_json", ExportJsonAsync),
                new(this,"Xlsx", "mdi:mdi-microsoft-excel", ExportXlsxAsync),
            ];
        }

        private Task ExportTxtAsync() => Export(DiaryFileManager.ExportTxtAsync, ExportKind.Txt);

        private Task ExportJsonAsync() => Export(DiaryFileManager.ExportJsonAsync, ExportKind.Json);

        private Task ExportMdAsync() => Export(DiaryFileManager.ExportMdAsync, ExportKind.Md);

        private Task ExportXlsxAsync() => Export(DiaryFileManager.ExportXlsxAsync, ExportKind.Xlsx);

        private async Task Export(Func<List<DiaryModel>, Task<string>> func, ExportKind exportKind)
        {
            await InternalVisibleChanged(false);

            var writePermission = await PlatformIntegration.TryStorageWritePermission();
            if (!writePermission)
            {
                await AlertService.InfoAsync(I18n.T("Please grant permission for storage writing"));
                return;
            }

            AlertService.StartLoading();
            try
            {
                var diaries = Value.OrderBy(x => x.CreateTime).ToList();
                string path = await func(diaries);
                if (!string.IsNullOrEmpty(path))
                {
                    string fileName = DiaryFileManager.GetExportFileName(exportKind);
                    bool flag = await PlatformIntegration.SaveFileAsync(fileName, path);
                    if (flag)
                    {
                        await AlertService.SuccessAsync(I18n.T("Export successfully"));
                        await HandleAchievements(Achievement.Export);
                    }
                }
            }
            catch (Exception e)
            {
                Logger.LogError(new EventId(1, "Error"), e, "Create file wrong");
                await AlertService.ErrorAsync(I18n.T("Export failed"));
            }
            finally
            {
                AlertService.StopLoading();
            }
        }
    }
}
