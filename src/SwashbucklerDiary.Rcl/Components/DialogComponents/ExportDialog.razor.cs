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
                new(this,"Txt", "mdi-format-text", ExportTxtAsync),
                new(this,"Md", "mdi-language-markdown-outline", ExportMdAsync),
                new(this,"Json", "mdi-code-braces", ExportJsonAsync),
                new(this,"Xlsx", "mdi-table-large", ExportXlsxAsync),
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
                await PopupServiceHelper.Info(I18n.T("Permission.OpenStorageWrite"));
                return;
            }

            await PopupServiceHelper.StartLoading();
            _ = Task.Run(async () =>
            {
                try
                {
                    string path = await func(Value);
                    if (!string.IsNullOrEmpty(path))
                    {
                        string fileName = DiaryFileManager.GetExportFileName(exportKind);
                        bool flag = await PlatformIntegration.SaveFileAsync(fileName, path);
                        if (flag)
                        {
                            await PopupServiceHelper.Success(I18n.T("Export.Export.Success"));
                            await HandleAchievements(Achievement.Export);
                        }
                    }

                }
                catch (Exception e)
                {
                    Logger.LogError(new EventId(1, "Error"), e, "Create file wrong");
                    await PopupServiceHelper.Error(I18n.T("Export.Export.Fail"));
                }
                finally
                {
                    await PopupServiceHelper.StopLoading();
                }
            });

        }
    }
}
