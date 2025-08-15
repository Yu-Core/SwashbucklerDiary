using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class ExportByTimeDialog : DialogComponentBase
    {
        private bool showExport;

        private List<DynamicListItem> exportTimeItems = [];

        private List<DiaryModel> exportDiaries = [];

        [Inject]
        protected IDiaryService DiaryService { get; set; } = default!;

        [Parameter]
        public DateOnly Value { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LoadView();
        }

        private void LoadView()
        {
            exportTimeItems =
            [
                new(this,"Export this day diaries","mdi:mdi-alpha-d",ExportThisDay),
                new(this,"Export this month diaries","mdi:mdi-alpha-m",ExportThisMonth),
                new(this,"Export this year diaries","mdi:mdi-alpha-y",ExportThisYear),
            ];
        }

        private Task ExportThisDay()
            => ExportThisTime("yyyy-MM-dd");

        private Task ExportThisMonth()
            => ExportThisTime("yyyy-MM");

        private Task ExportThisYear()
            => ExportThisTime("yyyy");

        private async Task ExportThisTime(string format)
        {
            await InternalVisibleChanged(false);
            await InvokeAsync(StateHasChanged);

            var dateTime = Value.ToDateTime(TimeOnly.MinValue);
            Expression<Func<DiaryModel, bool>> expression = 
                it=>it.CreateTime.ToString(format) == dateTime.ToString(format);
            exportDiaries = await DiaryService.QueryDiariesAsync(expression);
            if (exportDiaries.Count == 0)
            {
                await AlertService.InfoAsync(I18n.T("No diary"));
                return;
            }

            showExport = true;
        }
    }
}
