using BlazorComponent.JSInterop;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;
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

        private ScrollContainer scrollContainer = default!;

        private DateOnly[] eventsDates = [];

        private List<DiaryModel> selectedDiaries = [];

        private Guid datePickerKey = Guid.NewGuid();

        private List<DynamicListItem> menuItems = [];

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LoadView();
            NavigateService.BeforePopToRoot += BeforePopToRoot;
        }

        protected override void OnDispose()
        {
            NavigateService.BeforePopToRoot -= BeforePopToRoot;
            base.OnDispose();
        }

        protected override async Task UpdateDiariesAsync()
        {
            var diaries = await DiaryService.QueryAsync(it => !it.Private);
            Diaries = diaries;
            UpdateEventsDates(diaries);
            UpdateSelectedDiaries(diaries);
        }

        private void LoadView()
        {
            menuItems =
            [
                new(this, "History.Add diary","mdi-pencil", ()=>To($"write?CreateDate={SelectedDate}")),
                new(this, "History.Reset date","mdi-calendar-refresh", ResetDatePicker),
                new(this, "History.Export diaries","mdi-export", ()=>showExportThisTime = true),
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
                await JS.ScrollTo(scrollContainer.Ref, 0);
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
            selectedDiaries = diaries.Where(it
                => !it.Private
                && DateOnly.FromDateTime(it.CreateTime) == _selectedDate)
                .ToList();
        }

        private void HandleIntersectChanged(bool value)
        {
            if (IsCurrentPage)
            {
                normalCalendarVisible = value;
            }
        }

        private async Task BeforePopToRoot(PopEventArgs args)
        {
            if (thisPageUrl == args.PreviousUri && thisPageUrl == args.NextUri)
            {
                await JS.ScrollTo(scrollContainer.Ref, 0);
            }
        }

        private void ResetDatePicker()
        {
            datePickerKey = Guid.NewGuid();
            SelectedDate = DateOnly.FromDateTime(DateTime.Now);
        }
    }
}
