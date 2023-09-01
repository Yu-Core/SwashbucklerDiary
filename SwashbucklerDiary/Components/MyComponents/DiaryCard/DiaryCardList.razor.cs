using BlazorComponent;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class DiaryCardList : MyComponentBase
    {
        private List<DiaryModel> _value = default!;
        private List<DiaryModel> _internalValue = new();
        private bool ShowDeleteDiary;
        private bool ShowSelectTag;
        private bool ShowExport;
        private DiaryModel SelectedDiary = new();
        private List<DiaryModel> ExportDiaries = new();
        private int loadCount = 20;
        private bool firstLoad = true;

        [Inject]
        private IJSRuntime JS { get; set; } = default!;
        [Inject]
        public IDiaryService DiaryService { get; set; } = default!;
        [Inject]
        private IPlatformService PlatformService { get; set; } = default!;
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
        [Parameter]
        public string? NotFoundText { get; set; }

        public bool ShowPrivacy { get; set; }
        public bool ShowIcon { get; set; }
        public string? DateFormat { get; set; }

        public async Task Topping(DiaryModel diaryModel)
        {
            diaryModel.Top = !diaryModel.Top;
            diaryModel.UpdateTime = DateTime.Now;
            await InvokeAsync(StateHasChanged);
            await DiaryService.UpdateAsync(diaryModel);
        }

        public void Delete(DiaryModel diaryModel)
        {
            SelectedDiary = diaryModel;
            ShowDeleteDiary = true;
            InvokeAsync(StateHasChanged);
        }

        public async Task Copy(DiaryModel diaryModel)
        {
            var text = DiaryCopyContent(diaryModel);
            await PlatformService.SetClipboard(text);
            await AlertService.Success(I18n.T("Share.CopySuccess"));
        }

        public async Task ChangeTag(DiaryModel diary)
        {
            SelectedDiary = diary;
            SelectedTags = await DiaryService.GetTagsAsync(SelectedDiary.Id);
            await InvokeAsync(StateHasChanged);
            ShowSelectTag = true;
            await InvokeAsync(StateHasChanged);
        }


        public async Task MovePrivacy(DiaryModel diaryModel)
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
            await InvokeAsync(StateHasChanged);
            await OnUpdate.InvokeAsync();
        }

        public void Export(DiaryModel diary)
        {
            ExportDiaries = new() { diary };
            ShowExport = true;
            InvokeAsync(StateHasChanged);
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await base.OnInitializedAsync();
        }

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

        private async Task ConfirmDelete()
        {
            var diaryModel = SelectedDiary;
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
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                await AlertService.Error(I18n.T("Share.DeleteFail"));
            }
            await OnUpdate.InvokeAsync();
        }

        private async Task SaveSelectTags()
        {
            ShowSelectTag = false;
            SelectedDiary.UpdateTime = DateTime.Now;
            await DiaryService.UpdateTagsAsync(SelectedDiary);
        }

        private static string DiaryCopyContent(DiaryModel diary)
        {
            if (string.IsNullOrEmpty(diary.Title))
            {
                return diary.Content!;
            }
            return diary.Title + "\n" + diary.Content;
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
    }
}
