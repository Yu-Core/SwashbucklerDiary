using Microsoft.AspNetCore.Components;
using Serilog;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class ExportDialog : DialogComponentBase
    {
        private List<DynamicListItem> ListItemModels = new();

        [Inject]
        protected IPlatformService PlatformService { get; set; } = default!;
        [Inject]
        protected IAppDataService AppDataService { get; set; } = default!;

        [Parameter]
        public List<DiaryModel> Diaries { get; set; } = new();
        protected override void OnInitialized()
        {
            LoadView();
            base.OnInitialized();
        }

        private void LoadView()
        {
            ListItemModels = new()
            {
                new(this,"TXT","mdi-format-text",CreateTxtFile),
                new(this,"MD","mdi-language-markdown-outline",CreateMDFile),
                new(this,"JSON","mdi-code-json",CreateJsonFile),
            };
        }

        private Task CreateTxtFile() => CreateFile(AppDataService.ExportTxtZipFileAndSaveAsync);

        private Task CreateJsonFile() => CreateFile(AppDataService.ExportJsonZipFileAndSaveAsync);

        private Task CreateMDFile() => CreateFile(AppDataService.ExportMdZipFileAndSaveAsync);

        private async Task CreateFile(Func<List<DiaryModel>,Task<bool>> func)
        {
            await InternalValueChanged(false);
            await AlertService.StartLoading();
            try
            {
                bool flag = await func(Diaries);
                if (flag)
                {
                    await AlertService.Success(I18n.T("Export.Export.Success"));
                    await HandleAchievements(AchievementType.Export);
                }
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}\n{e.StackTrace}");
                await AlertService.Error(I18n.T("Export.Export.Fail"));
            }
            finally
            {
                await AlertService.StopLoading();
            }
        }
    }
}
