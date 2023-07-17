using Microsoft.JSInterop;
using SwashbucklerDiary.Components;

namespace SwashbucklerDiary.Pages
{
    public partial class HistoryPage : DiariesPageComponentBase
    {
        private ScrollContainer? scrollContainer;
        private bool NormalCalendarVisible = true;
        private bool ShowFloatCalendar;
        private DateOnly _pickedDate = DateOnly.FromDateTime(DateTime.Now);
        private DateOnly[] EventsDates = Array.Empty<DateOnly>();
        private IJSObjectReference? module;

        [JSInvokable]
        public async Task Show(bool value)
        {
            NormalCalendarVisible = value;
            await InvokeAsync(StateHasChanged);
        }

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await UpdateEventsDates();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if(firstRender)
            {
                module = await JS!.InvokeAsync<IJSObjectReference>("import", "./js/scroll.js");
                var dotNetCallbackRef = DotNetObjectReference.Create(this);
                await module.InvokeVoidAsync("ElementVisible", new object[4] { dotNetCallbackRef ,"Show", ".my-scroll-container", ".normal-calendar" });
            }
            
            await base.OnAfterRenderAsync(firstRender);
        }

        protected override async Task UpdateDiariesAsync()
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
            await UpdateDiariesAsync();
            await InvokeAsync(StateHasChanged);
            if(scrollContainer == null)
            {
                return;
            }

            await scrollContainer.ScrollToTop();
        }

        private async Task UpdateEventsDates()
        {
            var eventsDates = await DiaryService.GetAllDates(it=>!it.Private);
            EventsDates = eventsDates.ToArray();
        }

    }
}
