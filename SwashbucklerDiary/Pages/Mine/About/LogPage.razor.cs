using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Extend;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using System.Linq.Expressions;
using System.Text;

namespace SwashbucklerDiary.Pages
{
    public partial class LogPage : PageComponentBase
    {
        private bool ShowSearch;
        private bool ShowMenu;
        private bool ShowFilter;
        private bool ShowDelete;
        private bool ShowShare;
        private List<DynamicListItem> ListItemModels = new();
        private List<DynamicListItem> ShareItems = new();
        private List<LogModel> Logs = new();
        private string? Search;
        private DateFilterForm DateFilterForm = new();

        [Inject]
        private ILogService LogService { get; set; } = default!;
        [Inject]
        private IAppDataService AppDataService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            LoadView();
            await UpdateLogsAsync();
            await HandleAchievements(AchievementType.Log);
            await base.OnInitializedAsync();
        }

        protected override void NavigateToBack()
        {
            if (ShowSearch)
            {
                ShowSearch = false;
                return;
            }
            base.NavigateToBack();
        }

        private DateOnly DateOnlyMin => DateFilterForm.GetDateMinValue();
        private DateOnly DateOnlyMax => DateFilterForm.GetDateMaxValue();
        private bool IsSearchFiltered => !string.IsNullOrWhiteSpace(Search);
        private bool IsDateFiltered => DateOnlyMin != DateOnly.MinValue || DateOnlyMax != DateOnly.MaxValue;

        private void LoadView()
        {
            ListItemModels = new List<DynamicListItem>()
            {
                new(this, "Log.Clear","mdi-delete-outline",OpenDeleteDialog),
                new(this, "Share.Share","mdi-share-variant-outline",OpenShareDialog),
            };

            ShareItems = new()
            {
                new(this, "Share.TextShare","mdi-format-text",ShareText),
                new(this, "Share.FileShare","mdi-file-outline",ShareLogFile),
            };
        }

        private async Task OpenShareDialog()
        {
            if (!Logs.Any())
            {
                await AlertService.Info(I18n.T("Log.NoLog"));
                return;
            }

            ShowShare = true;
            StateHasChanged();
        }

        private async Task<string> Share()
        {
            ShowShare = false;
            if (!Logs.Any())
            {
                await AlertService.Info(I18n.T("Log.NoLog"));
                return string.Empty;
            }

            StringBuilder text = new();
            foreach (var item in Logs)
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

            await PlatformService.ShareText(I18n.T("Share.Share")!, text);
        }

        private async Task ShareLogFile()
        {
            var text = await Share();
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var fn = "Logs.txt";
            string path = await AppDataService.CreateCacheFileAsync(fn, text);
            await PlatformService.ShareFile(I18n.T("Log.Share")!, path);
        }

        private void OpenDeleteDialog()
        {
            ShowDelete = true;
            StateHasChanged();
        }

        private async Task HandleDelete()
        {
            ShowDelete = false;
            bool flag = await LogService.DeleteAsync();
            if (flag)
            {
                Logs = new();
                await AlertService.Success(I18n.T("Share.DeleteSuccess"));
            }
            else
            {
                await AlertService.Error(I18n.T("Share.DeleteFail"));
            }
        }

        private async Task UpdateLogsAsync()
        {
            Expression<Func<LogModel, bool>> func = Func();
            var logs = await LogService.QueryAsync(func);
            Logs = logs.OrderByDescending(it => it.Timestamp).ToList();
        }

        private Expression<Func<LogModel, bool>> Func()
        {
            Expression<Func<LogModel, bool>> expSearch;
            Expression<Func<LogModel, bool>> expDate;
            expSearch = it => (it.RenderedMessage ?? string.Empty).ToLower().Contains((Search ?? string.Empty).ToLower());

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
