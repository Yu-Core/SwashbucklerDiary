using Microsoft.AspNetCore.Components;
using Serilog;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using SwashbucklerDiary.Shared;

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
            await AlertService.StartLoading();
            try
            {
                bool flag = await AppDataService.ExportTxtZipFileAndSaveAsync(Diaries);
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
            finally
            {
                await AlertService.StopLoading();
            }
        }


        private async Task CreateJsonFile()
        {
            await InternalValueChanged(false);
            await AlertService.StartLoading();
            try
            {
                bool flag = await AppDataService.ExportJsonZipFileAndSaveAsync(Diaries);
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
            finally
            {
                await AlertService.StopLoading();
            }
        }

        private static Task CreatePDFFile()
        {
            return Task.CompletedTask;
        }

        private async Task CreateMDFile()
        {
            await InternalValueChanged(false);
            await AlertService.StartLoading();
            try
            {
                bool flag = await AppDataService.ExportMdZipFileAndSaveAsync(Diaries);
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
            finally
            {
                await AlertService.StopLoading();
            }
        }


    }
}
