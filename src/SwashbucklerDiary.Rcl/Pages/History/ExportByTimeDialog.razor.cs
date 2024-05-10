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

        private Expression<Func<DiaryModel, bool>> ExpressionDay =>
            it => it.CreateTime.Day == Value.Day &&
            it.CreateTime.Month == Value.Month &&
            it.CreateTime.Year == Value.Year;

        private Expression<Func<DiaryModel, bool>> ExpressionMonth =>
            it => it.CreateTime.Month == Value.Month &&
            it.CreateTime.Year == Value.Year;

        private Expression<Func<DiaryModel, bool>> ExpressionYear =>
            it => it.CreateTime.Year == Value.Year;

        private void LoadView()
        {
            exportTimeItems =
            [
                new(this,"History.ExportTime.Day","mdi-alpha-d",ExportThisDay),
                new(this,"History.ExportTime.Month","mdi-alpha-m",ExportThisMonth),
                new(this,"History.ExportTime.Year","mdi-alpha-y",ExportThisYear),
            ];
        }

        private Task ExportThisDay()
            => ExportThisTime(ExpressionDay);

        private Task ExportThisMonth()
            => ExportThisTime(ExpressionMonth);

        private Task ExportThisYear()
            => ExportThisTime(ExpressionYear);

        private async Task ExportThisTime(Expression<Func<DiaryModel, bool>> expression)
        {
            await InternalVisibleChanged(false);
            await InvokeAsync(StateHasChanged);

            exportDiaries = await DiaryService.QueryAsync(expression);
            if (exportDiaries.Count == 0)
            {
                await AlertService.Info(I18n.T("Diary.NoDiary"));
                return;
            }

            showExport = true;
        }
    }
}
