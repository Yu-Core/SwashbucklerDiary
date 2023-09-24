using BlazorComponent.JSInterop;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using System.Linq.Expressions;

namespace SwashbucklerDiary.Pages
{
    public partial class IndexHistory : ImportantComponentBase
    {
        private bool NormalCalendarVisible = true;
        private bool ShowFloatCalendar;
        private bool ShowExport;
        private bool ShowExportTime;
        private ScrollContainer ScrollContainer = default!;
        private ElementReference NormalCalendar;
        private DateOnly _pickedDate = DateOnly.FromDateTime(DateTime.Now);
        private DateOnly[] EventsDates = Array.Empty<DateOnly>();
        private List<DynamicListItem> ExportTimeItems = new();
        private List<DiaryModel> ExportDiaries = new();

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

        protected override void OnInitialized()
        {
            LoadView();
            base.OnInitialized();
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

        private List<DiaryModel> PickerDiaries => Diaries.Where(it => !it.Private && it.CreateTime >= MinPickedDateTime && it.CreateTime <= MaxPickedDateTime).ToList();

        private DateOnly PickedDate
        {
            get => _pickedDate;
            set => SetPickedDate(value);
        }

        private DateTime MinPickedDateTime => _pickedDate.ToDateTime(default);

        private DateTime MaxPickedDateTime => _pickedDate.ToDateTime(TimeOnly.MaxValue);

        private Expression<Func<DiaryModel, bool>> ExpressionDay =>
            it => it.CreateTime.Day == PickedDate.Day &&
            it.CreateTime.Month == PickedDate.Month &&
            it.CreateTime.Year == PickedDate.Year;

        private Expression<Func<DiaryModel, bool>> ExpressionMonth =>
            it => it.CreateTime.Month == PickedDate.Month &&
            it.CreateTime.Year == PickedDate.Year;

        private Expression<Func<DiaryModel, bool>> ExpressionYear =>
            it => it.CreateTime.Year == PickedDate.Year;

        

        private void LoadView()
        {
            ExportTimeItems = new()
            {
                new(this,"History.ExportTime.Day","mdi-alpha-d",ExportThisDay),
                new(this,"History.ExportTime.Month","mdi-alpha-m",ExportThisMonth),
                new(this,"History.ExportTime.Year","mdi-alpha-y",ExportThisYear),
            };
        }

        private async void SetPickedDate(DateOnly value)
        {
            if (_pickedDate == value)
            {
                return;
            }

            _pickedDate = value;
            //await UpdateDiariesAsync();
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

        private Task ExportThisDay()
            => ExportThisTime(ExpressionDay);

        private Task ExportThisMonth()
            => ExportThisTime(ExpressionMonth);

        private Task ExportThisYear()
            => ExportThisTime(ExpressionYear);

        private async Task ExportThisTime(Expression<Func<DiaryModel, bool>> expression)
        {
            ShowExportTime = false;
            var flag = await CheckPermission();
            if (!flag)
            {
                return;
            }

            ExportDiaries = await DiaryService.QueryAsync(expression);
            if (!ExportDiaries.Any())
            {
                await AlertService.Info(I18n.T("Diary.NoDiary"));
                return;
            }

            ShowExport = true;
            StateHasChanged();
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
