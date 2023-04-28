using SwashbucklerDiary.Components;

namespace SwashbucklerDiary.Pages
{
    public partial class HistoryPage : DiariesPageComponentBase
    {
        private DateOnly PickedDate = DateOnly.FromDateTime(DateTime.Now);
        private DateOnly _pickedDate = DateOnly.FromDateTime(DateTime.Now);
        private DateOnly[] EventsDates = Array.Empty<DateOnly>();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await UpdateEventsDates();
        }

        protected override async Task UpdateDiaries()
        {
            Diaries = await DiaryService.QueryAsync(it => !it.Private && it.CreateTime >= MinPickedDateTime && it.CreateTime <= MaxPickedDateTime);
        }

        private DateOnly PickedDate
        {
            get => _pickedDate;
            set => SetPickedDate(value);
        }

        public DateTime MinPickedDateTime => _pickedDate.ToDateTime(default);

        public DateTime MaxPickedDateTime => _pickedDate.ToDateTime(TimeOnly.MaxValue);

        private async void SetPickedDate(DateOnly value)
        {
            if(_pickedDate == value)
            {
                return;
            }

            _pickedDate = value;
            await UpdateDiaries();
            await InvokeAsync(StateHasChanged);
        }

        private async Task UpdateEventsDates()
        {
            var eventsDates = await DiaryService.GetAllDates(it=>!it.Private);
            EventsDates = eventsDates.ToArray();
        }
    }
}
