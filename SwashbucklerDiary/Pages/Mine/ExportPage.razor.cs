using Microsoft.AspNetCore.Components;
using Serilog;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using System.Text.Json;

namespace SwashbucklerDiary.Pages
{
    public partial class ExportPage : PageComponentBase
    {
        private bool ShowExport;
        private bool ShowImport;
        private string? ImportFilePath;
        private List<DiaryModel> Diaries = new();

        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            Diaries = await DiaryService.QueryIncludesAsync();
            await base.OnInitializedAsync();
        }

        private async Task Export()
        {
            var flag = await CheckPermission();
            if (!flag)
            {
                return;
            }

            if(!Diaries.Any())
            {
                await AlertService.Info(I18n.T("Diary.NoDiary"));
                return;
            }

            ShowExport = true;
        }

        private async Task Import()
        {
            var flag = await CheckPermission();
            if (!flag)
            {
                return;
            }

            ImportFilePath = string.Empty;
            ImportFilePath = await SystemService.PickJsonFileAsync();
            if (string.IsNullOrEmpty(ImportFilePath))
            {
                return;
            }
            ShowImport = true;
        }

        private async Task<bool> CheckPermission()
        {
            var writePermission = await SystemService.TryStorageWritePermission();
            if (!writePermission)
            {
                await AlertService.Error(I18n.T("Permission.OpenStorageWrite"));
                return false;
            }

            var readPermission = await SystemService.TryStorageReadPermission();
            if (!readPermission)
            {
                await AlertService.Success(I18n.T("Permission.OpenStorageRead"));
                return false;
            }

            return true;
        }

        private async Task ConfirmImport()
        {
            ShowImport = false;
            if (string.IsNullOrEmpty(ImportFilePath))
            {
                await AlertService.Success(I18n.T("Export.ImportFail"));
                return;
            }

            try
            {
                using FileStream openStream = File.OpenRead(ImportFilePath);
                List<DiaryModel>? diaries = await JsonSerializer.DeserializeAsync<List<DiaryModel>>(openStream);
                if (diaries == null || !diaries.Any())
                {
                    await AlertService.Success(I18n.T("Export.ImportFail"));
                    return;
                }

                await DiaryService.ImportAsync(diaries);
                await AlertService.Success(I18n.T("Export.ImportSuccess"));
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}\n{e.StackTrace}");
                await AlertService.Success(I18n.T("Export.ImportFail"));
            }
            
        }
    }
}
