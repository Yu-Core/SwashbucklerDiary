using BlazorComponent.JSInterop;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class HistoryPage : DiariesPageComponentBase
    {
        private bool NormalCalendarVisible = true;

        private bool ShowFloatCalendar;

        private bool ShowExportThisTime;

        private ScrollContainer ScrollContainer = default!;

        private DateOnly _pickedDate = DateOnly.FromDateTime(DateTime.Now);

        private DateOnly[] EventsDates = Array.Empty<DateOnly>();

        private List<DiaryModel> PickedDiaries = new();

        protected override async Task UpdateDiariesAsync()
        {
            var diaries = await DiaryService.QueryAsync(it => !it.Private);
            Diaries = diaries;
            UpdateEventsDates(diaries);
            UpdatePickedDiaries(diaries);
        }

        private DateOnly PickedDate
        {
            get => _pickedDate;
            set => SetPickedDate(value);
        }

        private void SetPickedDate(DateOnly value)
        {
            if (_pickedDate == value)
            {
                return;
            }

            _pickedDate = value;
            UpdatePickedDiaries(Diaries);
            Task.Run(async () =>
            {
                await InvokeAsync(StateHasChanged);
                //直接滚动显得很生硬，所以延时0.2s
                await Task.Delay(200);
                await JS.ScrollTo(ScrollContainer.Ref, 0);
            });
        }

        private async Task ExportThisTime()
        {
            var flag = await PlatformService.TryStorageWritePermission();
            if (!flag)
            {
                return;
            }

            ShowExportThisTime = true;
        }

        private void HandelOnRemove(DiaryModel diary)
        {
            Diaries.Remove(diary);
            UpdateEventsDates(Diaries);
        }

        private void UpdateEventsDates(List<DiaryModel> diaries)
        {
            EventsDates = diaries.Select(s => DateOnly.FromDateTime(s.CreateTime))
                   .Distinct()
                   .ToArray();
        }

        private void UpdatePickedDiaries(List<DiaryModel> diaries)
        {
            PickedDiaries = diaries.Where(it
                => !it.Private
                && it.CreateTime.Day == PickedDate.Day
                && it.CreateTime.Month == PickedDate.Month
                && it.CreateTime.Year == PickedDate.Year)
                .ToList();
        }

        private void HandleIntersectChanged(bool value)
        {
            NormalCalendarVisible = value;
        }
    }
}
