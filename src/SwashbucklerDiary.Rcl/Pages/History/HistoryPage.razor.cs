using Masa.Blazor.Extensions;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class HistoryPage : DiariesPageComponentBase
    {
        private DateOnly _selectedDate = DateOnly.FromDateTime(DateTime.Now);

        private bool normalCalendarVisible = true;

        private bool showFloatCalendar;

        private bool showExportThisTime;

        private bool showMenu;

        private bool showConfirmMerge;

        private readonly string scrollContainerId = $"scroll-container-{Guid.NewGuid():N}";

        private string scrollContainerSelector = string.Empty;

        private DateOnly[] eventsDates = [];

        private List<DiaryModel> selectedDiaries = [];

        private Guid datePickerKey = Guid.NewGuid();

        private List<DynamicListItem> menuItems = [];

        protected override void OnInitialized()
        {
            base.OnInitialized();

            scrollContainerSelector = $"#{scrollContainerId}";
            LoadView();
        }

        protected override async Task UpdateDiariesAsync()
        {
            await base.UpdateDiariesAsync();
            UpdateEventsDates(Diaries);
            UpdateSelectedDiaries(Diaries);
        }

        private void LoadView()
        {
            menuItems =
            [
                new(this, "History.Export diaries","mdi-export", ()=>showExportThisTime = true),
                new(this, "History.Merge diaries.Title","mdi-axis-z-arrow", ()=>showConfirmMerge = true),
            ];
        }

        private DateOnly SelectedDate
        {
            get => _selectedDate;
            set => SetSelectedDate(value);
        }

        private void SetSelectedDate(DateOnly value)
        {
            if (_selectedDate == value)
            {
                return;
            }

            _selectedDate = value;
            UpdateSelectedDiaries(Diaries);
            StateHasChanged();
            Task.Run(async () =>
            {
                //直接滚动显得很生硬，所以延时0.2s
                await Task.Delay(200);
                await JS.ScrollTo(scrollContainerSelector, 0);
            });
        }

        private void HandelOnRemove(DiaryModel diary)
        {
            Diaries.Remove(diary);
            UpdateEventsDates(Diaries);
        }

        private void UpdateEventsDates(List<DiaryModel> diaries)
        {
            eventsDates = diaries.Select(s => DateOnly.FromDateTime(s.CreateTime))
                   .Distinct()
                   .ToArray();
        }

        private void UpdateSelectedDiaries(List<DiaryModel> diaries)
        {
            selectedDiaries = diaries.Where(it => DateOnly.FromDateTime(it.CreateTime) == _selectedDate).ToList();
        }

        private void HandleIntersectChanged(bool value)
        {
            if (IsThisPage)
            {
                normalCalendarVisible = value;
            }
        }

        private void ResetDatePicker()
        {
            datePickerKey = Guid.NewGuid();
            SelectedDate = DateOnly.FromDateTime(DateTime.Now);
        }

        private async void ConfirmMerge()
        {
            showConfirmMerge = false;
            if (selectedDiaries.Count < 2)
            {
                return;
            }

            await PopupServiceHelper.StartLoading();

            var diaries = selectedDiaries.OrderBy(it => it.CreateTime).ToList();
            string content = string.Join("\n", diaries.Select(it => it.Content));
            var firstDiary = diaries.First();
            firstDiary.Content = content;
            diaries.Remove(firstDiary);
            await DiaryService.UpdateAsync(firstDiary, it => it.Content!);
            await DiaryService.DeleteAsync(diaries);
            await UpdateDiariesAsync();

            await PopupServiceHelper.StopLoading();
            StateHasChanged();
        }

        private void ToWrite()
            => To($"write?CreateDate={SelectedDate}");
    }
}
