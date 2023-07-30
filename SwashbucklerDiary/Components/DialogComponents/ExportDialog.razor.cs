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
                //new(this,"PDF","mdi-file-pdf-box",ToDo),
            };
        }

        private async Task CreateTxtFile()
        {
            await InternalValueChanged(false);

            try
            {
                bool flag = await AppDataService.ExportTxtFileAndSaveAsync(Diaries);
                if (flag)
                {
                    //此处未来可能引入成就系统
                    await AlertService.Success(I18n.T("Export.Export.Success"));
                }
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}\n{e.StackTrace}");
                await AlertService.Error(I18n.T("Export.Export.Fail"));
            }
        }


        private async Task CreateJsonFile()
        {
            await InternalValueChanged(false);

            try
            {
                bool flag = await AppDataService.ExportJsonFileAndSaveAsync(Diaries);
                if (flag)
                {
                    await AlertService.Success(I18n.T("Export.Export.Success"));
                }
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}\n{e.StackTrace}");
                await AlertService.Error(I18n.T("Export.Export.Fail"));
            }
        }

        private static Task CreatePDFFile()
        {
            return Task.CompletedTask;
        }

        private async Task CreateMDFile()
        {
            await InternalValueChanged(false);

            try
            {
                bool flag = await AppDataService.ExportMdFileAndSaveAsync(Diaries);
                if (flag)
                {
                    await AlertService.Success(I18n.T("Export.Export.Success"));
                }
            }
            catch (Exception e)
            {
                Log.Error($"{e.Message}\n{e.StackTrace}");
                await AlertService.Error(I18n.T("Export.Export.Fail"));
            }

        }


    }
}
