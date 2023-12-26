using Microsoft.AspNetCore.Components;
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
        private IAppFileManager AppFileManager { get; set; } = default!;

        protected override void OnInitialized()
        {
            LoadView();
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            await UpdateLogsAsync();
            await HandleAchievements(Achievement.Log);
            await base.OnInitializedAsync();
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
        private bool IsDateFiltered => DateOnlyMin != DateOnly.MinValue || DateOnlyMax != DateOnly.MaxValue;

        private void LoadView()
        {
            menuItems = new List<DynamicListItem>()
            {
                new(this, "Log.Clear","mdi-delete-outline",OpenDeleteDialog),
                new(this, "Share.Share","mdi-share-variant-outline",OpenShareDialog),
            };

            shareItems = new()
            {
                new(this, "Share.TextShare","mdi-format-text",ShareText),
                new(this, "Share.FileShare","mdi-file-outline",ShareLogFile),
            };
        }

        private async Task OpenShareDialog()
        {
            if (logs.Count == 0)
            {
                await AlertService.Info(I18n.T("Log.NoLog"));
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
                await AlertService.Info(I18n.T("Log.NoLog"));
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

            await PlatformIntegration.ShareTextAsync(I18n.T("Share.Share")!, text);
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
            await PlatformIntegration.ShareFileAsync(I18n.T("Log.Share")!, path);
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
                await AlertService.Success(I18n.T("Share.DeleteSuccess"));
            }
            else
            {
                await AlertService.Error(I18n.T("Share.DeleteFail"));
            }
        }

        private async Task UpdateLogsAsync()
        {
            Expression<Func<LogModel, bool>> exp = GetExpression();
            var logs = await LogService.QueryAsync(exp);
            this.logs = logs.OrderByDescending(it => it.Timestamp).ToList();
        }

        private Expression<Func<LogModel, bool>> GetExpression()
        {
            Expression<Func<LogModel, bool>> expSearch;
            Expression<Func<LogModel, bool>> expDate;
            expSearch = it => (it.RenderedMessage ?? string.Empty).ToLower().Contains((search ?? string.Empty).ToLower());

            DateTime MinDateTime = DateOnlyMin.ToDateTime(default);
            DateTime MaxDateTime = DateOnlyMax.ToDateTime(TimeOnly.MaxValue);
            if (DateOnlyMax != DateOnly.MaxValue)
            {
                MaxDateTime = MaxDateTime.AddDays(1);
            }

            expDate = it => it.Timestamp >= MinDateTime && it.Timestamp <= MaxDateTime;
            return expDate.AndIF(IsSearchFiltered, expSearch);
        }
    }
}
