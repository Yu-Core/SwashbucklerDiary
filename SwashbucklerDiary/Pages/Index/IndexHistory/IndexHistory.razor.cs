using BlazorComponent.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class IndexHistory : ImportantComponentBase
    {
        private bool NormalCalendarVisible = true;
        private bool ShowFloatCalendar;
        private bool ShowExportThisTime;
        private ScrollContainer ScrollContainer = default!;
        private ElementReference NormalCalendar;
        private DateOnly _pickedDate = DateOnly.FromDateTime(DateTime.Now);
        private DateOnly[] EventsDates = Array.Empty<DateOnly>();

        [Inject]
        protected IDiaryService DiaryService { get; set; } = default!;

        [Parameter]
        public List<DiaryModel> Diaries { get; set; } = new();
        [Parameter]
        public List<TagModel> Tags { get; set; } = new();
        [Parameter]
        public EventCallback<List<TagModel>> TagsChanged { get; set; }

        [JSInvokable]
        public async Task ShowNormalCalendar(bool value)
        {
            NormalCalendarVisible = value;
            await InvokeAsync(StateHasChanged);
        }

        protected override async Task OnInitializedAsync()
        {
            await UpdateEventsDates();
            await base.OnInitializedAsync();
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

        protected override async void OnResume()
        {
            await UpdateEventsDates();
            base.OnResume();
        }

        private List<DiaryModel> PickerDiaries => 
            Diaries.Where(it => !it.Private && 
            it.CreateTime.Day == PickedDate.Day &&
            it.CreateTime.Month == PickedDate.Month &&
            it.CreateTime.Year == PickedDate.Year).ToList();

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

            Task.Run(async() =>
            {
                await InvokeAsync(StateHasChanged);
                //直接滚动显得很生硬，所以延时0.2s
                await Task.Delay(200);
                await JS.ScrollTo(ScrollContainer.Ref, 0);
            });
        }

        private async Task UpdateEventsDates()
        {
            var eventsDates = await DiaryService.GetAllDates(it => !it.Private);
            EventsDates = eventsDates.ToArray();
        }

        private async Task HandleOnRemove(DiaryModel diary)
        {
            Diaries.Remove(diary);
            await UpdateEventsDates();
        }

        private async Task ExportThisTime()
        {
            var flag = await CheckPermission();
            if (!flag)
            {
                return;
            }

            ShowExportThisTime = true;
        }

        private async Task<bool> CheckPermission()
        {
            var writePermission = await PlatformService.TryStorageWritePermission();
            if (!writePermission)
            {
                await AlertService.Info(I18n.T("Permission.OpenStorageWrite"));
                return false;
            }

            var readPermission = await PlatformService.TryStorageReadPermission();
            if (!readPermission)
            {
                await AlertService.Info(I18n.T("Permission.OpenStorageRead"));
                return false;
            }

            return true;
        }
    }
}
