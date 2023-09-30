using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Extensions;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Pages
{
    public partial class ExportThisTimeDialog : DialogComponentBase
    {
        private bool ShowExport;

        private List<DynamicListItem> ExportTimeItems = new();

        private List<DiaryModel> ExportDiaries = new();

        [Inject]
        protected IDiaryService DiaryService { get; set; } = default!;

        [Parameter]
        public DateOnly PickedDate { get; set; }

        protected override void OnInitialized()
        {
            LoadView();
            base.OnInitialized();
        }

        private Expression<Func<DiaryModel, bool>> ExpressionDay =>
            it => it.CreateTime.Day == PickedDate.Day &&
            it.CreateTime.Month == PickedDate.Month &&
            it.CreateTime.Year == PickedDate.Year;

        private Expression<Func<DiaryModel, bool>> ExpressionMonth =>
            it => it.CreateTime.Month == PickedDate.Month &&
            it.CreateTime.Year == PickedDate.Year;

        private Expression<Func<DiaryModel, bool>> ExpressionYear =>
            it => it.CreateTime.Year == PickedDate.Year;

        private void LoadView()
        {
            ExportTimeItems = new()
            {
                new(this,"History.ExportTime.Day","mdi-alpha-d",ExportThisDay),
                new(this,"History.ExportTime.Month","mdi-alpha-m",ExportThisMonth),
                new(this,"History.ExportTime.Year","mdi-alpha-y",ExportThisYear),
            };
        }

        private Task ExportThisDay()
            => ExportThisTime(ExpressionDay);

        private Task ExportThisMonth()
            => ExportThisTime(ExpressionMonth);

        private Task ExportThisYear()
            => ExportThisTime(ExpressionYear);

        private async Task ExportThisTime(Expression<Func<DiaryModel, bool>> expression)
        {
            await InternalValueChanged(false);
            await InvokeAsync(StateHasChanged);

            Expression<Func<DiaryModel, bool>> exp = it => !it.Private;
            exp =exp.And(expression);
            ExportDiaries = await DiaryService.QueryAsync(exp);
            if (!ExportDiaries.Any())
            {
                await AlertService.Info(I18n.T("Diary.NoDiary"));
                return;
            }

            ShowExport = true;
        }
    }
}
