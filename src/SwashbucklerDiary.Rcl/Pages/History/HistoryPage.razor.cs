using Masa.Blazor;
using Masa.Blazor.Extensions;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Shared;
using System.Globalization;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class HistoryPage : DiariesPageComponentBase
    {
        private bool normalCalendarVisible = true;

        private bool showFloatCalendar;

        private bool showExportThisTime;

        private bool showMenu;

        private bool showConfirmMerge;

        private int firstDayOfWeek;

        private readonly string scrollContainerId = $"scroll-container-{Guid.NewGuid():N}";

        private string scrollContainerSelector = string.Empty;

        private DateOnly[] eventsDates = [];

        private Guid datePickerKey = Guid.NewGuid();

        private List<DynamicListItem> menuItems = [];

        protected override List<DiaryModel> Diaries
        {
            get => GetValue<List<DiaryModel>>() ?? [];
            set => SetValue(value);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            scrollContainerSelector = $"#{scrollContainerId}";
            LoadView();
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            firstDayOfWeek = SettingService.Get(it => it.FirstDayOfWeek);
        }

        protected override void RegisterWatchers(PropertyWatcher watcher)
        {
            base.RegisterWatchers(watcher);

            watcher.Watch<DateOnly>(nameof(SelectedDate), () =>
                    {
                        SetValueWithNoEffect(GetSelectedDiaries(), nameof(SelectedDiaries));
                        StateHasChanged();
                        Task.Run(async () =>
                        {
                            //直接滚动显得很生硬，所以延时0.2s
                            await Task.Delay(200);
                            await JS.ScrollTo(scrollContainerSelector, 0);
                        });
                    }, immediate: true)
                    .Watch<List<DiaryModel>>(nameof(Diaries), () =>
                    {
                        SetValueWithNoEffect(GetSelectedDiaries(), nameof(SelectedDiaries));
                        UpdateEventsDates();
                    }, immediate: true)
                    .Watch<List<DiaryModel>>(nameof(SelectedDiaries), async () =>
                    {
                        var diaries = await DiaryService.QueryDiariesAsync();
                        SetValueWithNoEffect(diaries, nameof(Diaries));
                        UpdateEventsDates();
                        StateHasChanged();
                    });
        }

        private void LoadView()
        {
            menuItems =
            [
                new(this, "Export diaries","mdi:mdi-export", ()=>showExportThisTime = true),
                new(this, "Merge diaries","merge", ()=>showConfirmMerge = true),
            ];
        }

        private DateOnly SelectedDate
        {
            get => GetValue(DateOnly.FromDateTime(DateTime.Now));
            set => SetValue(value);
        }

        private List<DiaryModel> SelectedDiaries
        {
            get => GetValue<List<DiaryModel>>() ?? [];
            set => SetValue(value);
        }

        private List<DiaryModel> GetSelectedDiaries()
        {
            return Diaries.Where(it => DateOnly.FromDateTime(it.CreateTime) == SelectedDate).ToList();
        }

        private void UpdateEventsDates()
        {
            eventsDates = Diaries.Select(s => DateOnly.FromDateTime(s.CreateTime)).Distinct().ToArray();
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
            if (SelectedDiaries.Count < 2)
            {
                return;
            }

            await AlertService.StartLoading();

            var diaries = SelectedDiaries.OrderBy(it => it.CreateTime).ToList();
            string content = string.Join("\n", diaries.Select(it => it.Content));
            var firstDiary = diaries.First();
            firstDiary.Content = content;
            diaries.Remove(firstDiary);
            await DiaryService.UpdateAsync(firstDiary, it => it.Content!);
            await DiaryService.DeleteAsync(diaries);
            await UpdateDiariesAsync();

            await AlertService.StopLoading();
            StateHasChanged();
        }

        private void ToWrite()
            => To($"write?CreateDate={SelectedDate.ToString(CultureInfo.InvariantCulture)}");
    }
}
