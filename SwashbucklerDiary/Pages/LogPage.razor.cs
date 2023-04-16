using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Extend;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using System.Linq.Expressions;
using System.Text;

namespace SwashbucklerDiary.Pages
{
    public partial class LogPage : PageComponentBase, IDisposable
    {
        private bool _showSearch;
        private bool ShowMenu;
        private bool ShowFilter;
        private bool ShowDelete;
        private bool ShowShare;
        private List<LogModel> AllLogs = new();
        private string? Search;
        private List<ListItemModel> ListItemModels = new();
        private List<ListItemModel> ShareItems = new();
        private List<LogModel> Logs = new();
        private Expression<Func<LogModel, bool>>? _dateExpression = null;

        [Inject]
        private ILogService LogService { get; set; } = default!;

        public void Dispose()
        {
            if (ShowSearch)
            {
                NavigateService.Action -= CloseSearch;
            }
            GC.SuppressFinalize(this);
        }

        protected override async Task OnInitializedAsync()
        {
            LoadView();
            await InitLogs();
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

        private bool ShowSearch
        {
            get => _showSearch;
            set => SetShowSearch(value);
        }

        private Expression<Func<LogModel, bool>> DateExpression => GetDateExpression();
        private Expression<Func<LogModel, bool>> SearchExpression => GetSearchExpression();
        private bool IsFilter => _dateExpression != null;
        private async Task InitLogs()
        {
            var logs = await LogService.QueryAsync();
            AllLogs = Logs = logs.OrderByDescending(it => it.Timestamp).ToList();
        }

        private void LoadView()
        {
            ListItemModels = new List<ListItemModel>()
            {
                new("Log.Clear","mdi-delete-outline",OpenDeleteDialog),
                new("Share.Share","mdi-share-variant-outline",OpenShareDialog),
            };

            ShareItems = new()
            {
                new("Share.TextShare","mdi-format-text",ShareText),
                new("Share.FileShare","mdi-file-outline",ShareLogFile),
            };
        }

        private void OpenShareDialog()
        {
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

            await SystemService.ShareText(I18n.T("Share.Share")!, text);
        }

        private async Task ShareLogFile()
        {
            var text = await Share();
            if (string.IsNullOrEmpty(text))
            {
                return;
            }

            var targetFileName = "Logs.txt";
            string targetFile = Path.Combine(FileSystem.CacheDirectory, targetFileName);
            await File.WriteAllTextAsync(targetFile, text);
            await SystemService.ShareFile(I18n.T("Log.Share")!, targetFile);
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
                AllLogs = new();
                await AlertService.Success(I18n.T("Share.DeleteSuccess"));
            }
            else
            {
                await AlertService.Error(I18n.T("Share.DeleteFail"));
            }
        }

        private void SetShowSearch(bool value)
        {
            if (_showSearch != value)
            {
                if (value)
                {
                    NavigateService.Action += CloseSearch;
                }
                else
                {
                    Search = string.Empty;
                    NavigateService.Action -= CloseSearch;
                }
                _showSearch = value;
            }
        }

        private void CloseSearch()
        {
            ShowSearch = false;
            StateHasChanged();
        }

        private Expression<Func<LogModel, bool>> GetDateExpression()
        {
            if (_dateExpression == null)
            {
                Expression<Func<LogModel, bool>> exp = it => true;
                return exp;
            }

            return _dateExpression;
        }

        private Expression<Func<LogModel, bool>> GetSearchExpression()
        {
            Expression<Func<LogModel, bool>>? exp = null;
            if (!string.IsNullOrWhiteSpace(Search))
            {
                exp = exp.And(it => it.RenderedMessage!.Contains(Search));
            }
            else
            {
                exp = exp.And(it => true);
            }
            return exp!;
        }

        private void SearchChanged(string value)
        {
            Search = value;
            UpdateLogs();
        }

        private void UpdateLogs()
        {
            Func<LogModel, bool>? exp = DateExpression.Compile();
            Func<LogModel, bool>? exp2 = SearchExpression.Compile();
            Logs = AllLogs.Where(exp).Where(exp2).ToList();
        }
    }
}
