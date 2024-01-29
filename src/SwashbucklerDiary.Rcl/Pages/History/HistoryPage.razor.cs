using BlazorComponent.JSInterop;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class HistoryPage : DiariesPageComponentBase
    {
        private DateOnly _pickedDate = DateOnly.FromDateTime(DateTime.Now);

        private bool normalCalendarVisible = true;

        private bool showFloatCalendar;

        private bool showExportThisTime;

        private ScrollContainer scrollContainer = default!;

        private DateOnly[] eventsDates = [];

        private List<DiaryModel> pickedDiaries = [];

        protected override void OnInitialized()
        {
            base.OnInitialized();

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
            StateHasChanged();
            Task.Run(async () =>
            {
                //直接滚动显得很生硬，所以延时0.2s
                await Task.Delay(200);
                await JS.ScrollTo(scrollContainer.Ref, 0);
            });
        }

        private void ExportThisTime()
        {
            showExportThisTime = true;
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

        private void UpdatePickedDiaries(List<DiaryModel> diaries)
        {
            pickedDiaries = diaries.Where(it
                => !it.Private
                && it.CreateTime.Day == PickedDate.Day
                && it.CreateTime.Month == PickedDate.Month
                && it.CreateTime.Year == PickedDate.Year)
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
    }
}
