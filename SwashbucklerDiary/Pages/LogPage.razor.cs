using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using System.Text;

namespace SwashbucklerDiary.Pages
{
    public partial class LogPage : PageComponentBase
    {
        private bool _showSearch;
        private bool ShowMenu;
        private bool _showFilter;
        private bool ShowDelete;
        private bool ShowShare;
        private bool IsFilter;
        private List<LogModel> AllLogs = new();
        private List<LogModel> Logs = new();
        private string? Search;
        private List<ViewListItem> ViewListItems = new();
        private List<ViewListItem> ShareItems = new();

        [Inject]
        private ILogService LogService { get; set; } = default!;

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
        private bool ShowFilter
        {
            get => _showFilter;
            set => SetShowFilter(value);
        }

        private async Task InitLogs()
        {
            var logs = await LogService.QueryAsync();
            AllLogs = Logs = logs.OrderByDescending(it=>it.Timestamp).ToList();
        }

        private void LoadView()
        {
            ViewListItems = new List<ViewListItem>()
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

        private void TextChanged(string value)
        {
            Search = value;
            if (!string.IsNullOrWhiteSpace(Search))
            {
                Logs = AllLogs.Where(it=>it.RenderedMessage!.Contains(Search)).ToList();
            }
            else
            {
                IsFilter = false;
                Logs = AllLogs;
            }
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
                AllLogs = Logs = new();
                await AlertService.Success(I18n.T("Share.DeleteSuccess"));
            }
            else
            {
                await AlertService.Error(I18n.T("Share.DeleteFail"));
            }
        }

        private void FilterReset()
        {
            Logs = AllLogs;
        }

        private void FilterOK(List<LogModel> logs)
        {
            Logs = logs;
        }

        private void SetShowSearch(bool value)
        {
            if (_showSearch != value)
            {
                if (value)
                {
                    IsFilter = false;
                    FilterReset();
                }
                else
                {
                    Search = string.Empty;
                }
                _showSearch = value;
            }
        }

        private void SetShowFilter(bool value)
        {
            if (_showFilter != value)
            {
                if (value)
                {
                    ShowSearch = false;
                    FilterReset();
                }
                _showFilter = value;
            }
        }
    }
}
