using BlazorComponent;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class DiaryCardListContent : MyComponentBase, IDisposable
    {
        private List<DiaryModel> _value = default!;
        private List<DiaryModel> _internalValue = new();
        private bool ShowDeleteDiary;
        private bool ShowSelectTag;
        private bool ShowExport;
        private bool ShowPrivacy;
        private bool ShowIcon;
        private string? DateFormat;
        private DiaryModel SelectedDiary = new();
        private List<DiaryModel> ExportDiaries = new();
        private int loadCount = 20;
        //private double ScrollTop;
        private bool firstLoad = true;

        [Inject]
        private IJSRuntime JS { get; set; } = default!;
        [Inject]
        public IDiaryService DiaryService { get; set; } = default!;
        [Inject]
        private IPlatformService PlatformService { get; set; } = default!;
        [Inject]
        protected ISettingsService SettingsService { get; set; } = default!;
        [Inject]
        private ITagService TagService { get; set; } = default!;

        [CascadingParameter(Name = "ScrollElement")]
        public ElementReference ScrollElement { get; set; }
        [Parameter]
        public List<DiaryModel> Value
        {
            get => _value.OrderByDescending(it => it.Top).ToList();
            set => SetValue(value);
        }
        [Parameter]
        public string? Class { get; set; }
        [Parameter]
        public string? CardClass { get; set; }
        [Parameter]
        public EventCallback OnUpdate { get; set; }
        [Parameter]
        public List<TagModel> Tags { get; set; } = new();
        [Parameter]
        public EventCallback<List<TagModel>> TagsChanged { get; set; }

        public void Dispose()
        {
            //NavigateService.BeforeNavigate -= SetCache;
            GC.SuppressFinalize(this);
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            //NavigateService.BeforeNavigate += SetCache;
            await base.OnInitializedAsync();
        }

        //protected override void OnAfterRender(bool firstRender)
        //{
        //    if (firstRender)
        //    {
        //        LoadCache();
        //        Task.Run(async () =>
        //        {
        //            if (ScrollTop > 0)
        //            {
        //                await Task.Delay(500);
        //                await JS.InvokeVoidAsync("SetScrollTop", ScrollElement, ScrollTop);
        //            }
        //        });
        //    }

        //    base.OnAfterRender(firstRender);
        //}

        private List<DiaryModel> InternalValue
        {
            get => _internalValue.OrderByDescending(it => it.Top).ToList();
            set => _internalValue = value;
        }

        private List<TagModel> SelectedTags
        {
            get => SelectedDiary.Tags ?? new();
            set => SelectedDiary.Tags = value;
        }

        private bool ShowLoadMore => _internalValue.Any() && _internalValue.Count < _value.Count;

        private void SetValue(List<DiaryModel> value)
        {
            if (_value != value)
            {
                _value = value;
                _internalValue = new();
                _internalValue = MockRequest();
            }
        }

        private async Task LoadSettings()
        {
            ShowPrivacy = await SettingsService.Get(SettingType.PrivacyMode);
            ShowIcon = await SettingsService.Get(SettingType.DiaryCardIcon);
            DateFormat = await SettingsService.Get(SettingType.DiaryCardDateFormat);
        }

        private async Task HandleTopping(DiaryModel diaryModel)
        {
            diaryModel.Top = !diaryModel.Top;
            diaryModel.UpdateTime = DateTime.Now;
            await DiaryService.UpdateAsync(diaryModel);
        }

        private void OpenDeleteDialog(DiaryModel diaryModel)
        {
            SelectedDiary = diaryModel;
            ShowDeleteDiary = true;
        }

        private async Task HandleDelete(DiaryModel diaryModel)
        {
            ShowDeleteDiary = false;
            bool flag = await DiaryService.DeleteAsync(diaryModel);
            if (flag)
            {
                var index = _internalValue.FindIndex(it => it.Id == diaryModel.Id);
                if (index < 0)
                {
                    return;
                }
                _internalValue.RemoveAt(index);

                var index2 = _value.FindIndex(it => it.Id == diaryModel.Id);
                if (index2 < 0)
                {
                    return;
                }
                _value.RemoveAt(index2);
                await AlertService.Success(I18n.T("Share.DeleteSuccess"));
                StateHasChanged();
            }
            else
            {
                await AlertService.Error(I18n.T("Share.DeleteFail"));
            }
            await OnUpdate.InvokeAsync();
        }

        private async Task HandleCopy(DiaryModel diaryModel)
        {
            var text = DiaryCopyContent(diaryModel);
            await PlatformService.SetClipboard(text);

            await AlertService.Success(I18n.T("Share.CopySuccess"));
        }

        private async Task HandleTag(DiaryModel diary)
        {
            SelectedDiary = diary;
            SelectedTags = await DiaryService.GetTagsAsync(SelectedDiary.Id);
            StateHasChanged();
            ShowSelectTag = true;
        }

        private async Task SaveSelectTags()
        {
            ShowSelectTag = false;
            SelectedDiary.UpdateTime = DateTime.Now;
            await DiaryService.UpdateTagsAsync(SelectedDiary);
            Tags = await TagService.QueryAsync();
            if (TagsChanged.HasDelegate)
            {
                await TagsChanged.InvokeAsync(Tags);
            }
        }

        private void HandleClick(DiaryModel diaryModel)
        {
            NavigateService.NavigateTo($"/read/{diaryModel.Id}");
        }

        private static string DiaryCopyContent(DiaryModel diary)
        {
            if (string.IsNullOrEmpty(diary.Title))
            {
                return diary.Content!;
            }
            return diary.Title + "\n" + diary.Content;
        }

        private void OpenExportDialog(DiaryModel diary)
        {
            ExportDiaries = new() { diary };
            ShowExport = true;
        }

        private async Task HandlePrivacy(DiaryModel diaryModel)
        {
            diaryModel.Private = !diaryModel.Private;
            diaryModel.UpdateTime = DateTime.Now;
            await DiaryService.UpdateAsync(diaryModel);
            var index = _internalValue.FindIndex(it => it.Id == diaryModel.Id);
            if (index < 0)
            {
                return;
            }
            _internalValue.RemoveAt(index);

            var index2 = _value.FindIndex(it => it.Id == diaryModel.Id);
            if (index2 < 0)
            {
                return;
            }
            _value.RemoveAt(index2);
            await OnUpdate.InvokeAsync();
        }

        private void OnLoad(InfiniteScrollLoadEventArgs args)
        {
            if (firstLoad)
            {
                firstLoad = false;
                return;
            }

            var append = MockRequest();

            _internalValue.AddRange(append);
        }

        private List<DiaryModel> MockRequest()
        {
            return Value.Skip(_internalValue.Count).Take(loadCount).ToList();
        }

        //private void LoadCache()
        //{
        //    var internalValue = (List<DiaryModel>?)NavigateService.GetCurrentCache($"{nameof(DiaryCardListContent)}{nameof(InternalValue)}") ?? new List<DiaryModel>();
        //    if(internalValue.Count > 0)
        //    {
        //        InternalValue = internalValue;
        //        StateHasChanged();
        //    }

        //    ScrollTop = (double?)NavigateService.GetCurrentCache($"{nameof(DiaryCardListContent)}{nameof(ScrollTop)}") ?? 0;
        //}

        //private async Task SetCache()
        //{
        //    ScrollTop = await JS.InvokeAsync<double>("GetScrollTop", new object[1] { ScrollElement });
        //    NavigateService.SetCurrentCache($"{nameof(DiaryCardListContent)}{nameof(ScrollTop)}", ScrollTop);
        //    NavigateService.SetCurrentCache($"{nameof(DiaryCardListContent)}{nameof(InternalValue)}", InternalValue);
        //}
    }
}
