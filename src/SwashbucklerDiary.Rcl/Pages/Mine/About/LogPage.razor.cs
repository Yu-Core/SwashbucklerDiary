using Microsoft.AspNetCore.Components;
using SqlSugar;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;
using System.Linq.Expressions;
using System.Text;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class LogPage : ImportantComponentBase
    {
        private bool showSearch;

        private bool showMenu;

        private bool showFilter;

        private bool showDelete;

        private bool showShare;

        private List<DynamicListItem> menuItems = [];

        private List<DynamicListItem> shareItems = [];

        private List<LogModel> logs = [];

        private string? search;

        private DateFilterForm dateFilterForm = new();

        [Inject]
        private ILogService LogService { get; set; } = default!;

        [Inject]
        private IAppFileSystem AppFileManager { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LoadView();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await UpdateLogsAsync();
                await HandleAchievements(Achievement.Log);
                StateHasChanged();
            }
        }

        protected override Task NavigateToBack()
        {
            if (showSearch)
            {
                showSearch = false;
                return Task.CompletedTask;
            }

            return base.NavigateToBack();
        }

        private DateOnly DateOnlyMin => dateFilterForm.GetDateMinValue();

        private DateOnly DateOnlyMax => dateFilterForm.GetDateMaxValue();

        private bool IsSearchFiltered => !string.IsNullOrWhiteSpace(search);

        private bool IsDateFiltered => DateOnlyMin != default || DateOnlyMax != default;

        private void LoadView()
        {
            menuItems =
            [
                new(this, "Clear out", "mdi:mdi-delete-outline", OpenDeleteDialog),
                new(this, "Share", "share", OpenShareDialog),
            ];

            shareItems =
            [
                new(this, "Text sharing","description", ShareText),
                new(this, "File sharing","draft", ShareLogFile),
            ];
        }

        private async Task OpenShareDialog()
        {
            if (logs.Count == 0)
            {
                await AlertService.InfoAsync(I18n.T("No logs"));
                return;
            }

            showShare = true;
            StateHasChanged();
        }

        private async Task<string> Share()
        {
            showShare = false;
            if (logs.Count == 0)
            {
                await AlertService.InfoAsync(I18n.T("No logs"));
                return string.Empty;
            }

            StringBuilder text = new();
            foreach (var item in logs)
            {
                var itemText = item.Timestamp + " " + item.RenderedMessage;
                text.AppendLine(itemText);
            }

            return text.ToString();
        }

        private async Task ShareText()
        {
            var text = await Share();
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            await PlatformIntegration.ShareTextAsync(I18n.T("Share")!, text);
        }

        private async Task ShareLogFile()
        {
            var text = await Share();
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var fn = "Logs.txt";
            string path = await AppFileManager.CreateTempFileAsync(fn, text);
            await PlatformIntegration.ShareFileAsync(I18n.T("Share")!, path);
        }

        private void OpenDeleteDialog()
        {
            showDelete = true;
            StateHasChanged();
        }

        private async Task HandleDelete()
        {
            showDelete = false;
            StateHasChanged();
            bool flag = await LogService.DeleteAsync();
            if (flag)
            {
                logs = [];
                await AlertService.SuccessAsync(I18n.T("Delete successfully"));
            }
            else
            {
                await AlertService.ErrorAsync(I18n.T("Delete failed"));
            }
        }

        private async Task UpdateLogsAsync()
        {
            Expression<Func<LogModel, bool>> exp = CerateExpression();
            var logs = await LogService.QueryAsync(exp);
            this.logs = logs.OrderByDescending(it => it.Timestamp).ToList();
        }

        private Expression<Func<LogModel, bool>> CerateExpression()
        {
            var expable = Expressionable.Create<LogModel>();

            if (DateOnlyMin != default)
            {
                DateTime dateTimeMin = DateOnlyMin.ToDateTime(default);
                Expression<Func<LogModel, bool>> expMinDate = it => it.Timestamp >= dateTimeMin;
                expable.And(expMinDate);
            }

            if (DateOnlyMax != default)
            {
                DateTime dateTimeMax = DateOnlyMax.ToDateTime(TimeOnly.MaxValue);
                dateTimeMax = dateTimeMax.AddDays(1);
                Expression<Func<LogModel, bool>> expMaxDate = it => it.Timestamp <= dateTimeMax;
                expable.And(expMaxDate);
            }


            if (IsSearchFiltered)
            {
                Expression<Func<LogModel, bool>> expSearch
                    = it => (it.RenderedMessage ?? string.Empty).Contains(search ?? string.Empty, StringComparison.CurrentCultureIgnoreCase);
                expable.And(expSearch);
            }

            return expable.ToExpression();
        }
    }
}
