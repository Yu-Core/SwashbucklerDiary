using BlazorComponent.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Components;

namespace SwashbucklerDiary.Pages
{
    public partial class HistoryPage : DiariesPageComponentBase
    {
        private ScrollContainer ScrollContainer = default!;
        private ElementReference NormalCalendar;
        private bool NormalCalendarVisible = true;
        private bool ShowFloatCalendar;
        private DateOnly _pickedDate = DateOnly.FromDateTime(DateTime.Now);
        private DateOnly[] EventsDates = Array.Empty<DateOnly>();

        [JSInvokable]
        public async Task ShowNormalCalendar(bool value)
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
            if (firstRender)
            {
                var dotNetCallbackRef = DotNetObjectReference.Create(this);
                await JS.InvokeVoidAsync("listenElementVisibility", new object[4] { dotNetCallbackRef, "ShowNormalCalendar", ScrollContainer.Ref, NormalCalendar });
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
            if (_pickedDate == value)
            {
                return;
            }

            _pickedDate = value;
            await UpdateDiariesAsync();
            await InvokeAsync(StateHasChanged);
            //直接滚动显得很生硬，所以延时0.2s
            await Task.Delay(200);
            await JS.ScrollTo(ScrollContainer.Ref, 0);
        }

        private async Task UpdateEventsDates()
        {
            var eventsDates = await DiaryService.GetAllDates(it => !it.Private);
            EventsDates = eventsDates.ToArray();
        }

    }
}
