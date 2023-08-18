using BlazorComponent;
using Microsoft.AspNetCore.Components;
using OneOf;
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
        private bool ShowPrivacy;
        private bool ShowIcon;
        private string? DateFormat;
        private DiaryModel SelectedDiary = new();
        private List<DiaryModel> ExportDiaries = new();
        private int loadCount = 20;

        [Inject]
        public IDiaryService DiaryService { get; set; } = default!;
        [Inject]
        private IPlatformService PlatformService { get; set; } = default!;

        [Inject]
        protected ISettingsService SettingsService { get; set; } = default!;

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
        [Parameter]
        public OneOf<ElementReference, string>? ScrollParent { get; set; }

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

        private void SetValue(List<DiaryModel> value)
        {
            if (_value != value)
            {
                bool first = _value is null;
                _value = value;
                if (!first)
                {
                    _internalValue = new();
                    _internalValue = MockRequest();
                }
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
                if (index < 0)
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
            SelectedDiary.UpdateTime = DateTime.Now;
            await DiaryService.UpdateTagsAsync(SelectedDiary);
            ShowSelectTag = false;
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
            if (index < 0)
            {
                return;
            }
            _value.RemoveAt(index2);
            await OnUpdate.InvokeAsync();
        }

        private void OnLoad(InfiniteScrollLoadEventArgs args)
        {
            var append = MockRequest();

            _internalValue.AddRange(append);

            args.Status = InternalValue.Count == Value.Count ? InfiniteScrollLoadStatus.Empty : InfiniteScrollLoadStatus.Ok;
        }

        private List<DiaryModel> MockRequest()
        {
            return Value.Skip(_internalValue.Count).Take(loadCount).ToList();
        }
    }
}
