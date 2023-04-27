using SwashbucklerDiary.Components;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class HistoryPage : DiariesPageComponentBase
    {
        private DateOnly PickedDate = DateOnly.FromDateTime(DateTime.Now);
        private Func<DateOnly, bool> AllowedDates = PickedDate => true;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            UpdateAllowDates();
        }

        private List<DiaryModel> CalendarDiaries => Diaries.Where(it => DateOnly.FromDateTime(it.CreateTime) == PickedDate).ToList();

        private async Task Update()
        {
            await UpdateDiaries();
            UpdateAllowDates();
        }

        private void UpdateAllowDates()
        {
            var dateOnly = Diaries.Select(it => DateOnly.FromDateTime(it.CreateTime)).Distinct();
            AllowedDates = value => dateOnly.Contains(value);
        }
    }
}
