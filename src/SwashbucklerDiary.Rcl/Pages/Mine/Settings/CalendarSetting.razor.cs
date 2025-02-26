using SwashbucklerDiary.Rcl.Components;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class CalendarSetting : ImportantComponentBase
    {
        private bool showFirstDayOfWeek;

        private int firstDayOfWeek;

        private readonly Dictionary<string, int> firstDayOfWeekItems = [];

        protected override void OnInitialized()
        {
            base.OnInitialized();

            InitFirstDayOfWeekItems();
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            firstDayOfWeek = SettingService.Get(s => s.FirstDayOfWeek);
        }

        private void InitFirstDayOfWeekItems()
        {
            for (int i = 0; i < 7; i++)
            {
                firstDayOfWeekItems.Add($"Week.{i}", i);
            }
        }

        private async Task UpdateSetting()
        {
            await SettingService.SetAsync(s => s.FirstDayOfWeek, firstDayOfWeek);
        }

        private string FirstDayOfWeekText => I18n.T(firstDayOfWeekItems.FirstOrDefault(it => it.Value == firstDayOfWeek).Key);
    }
}