using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class DiaryCardList : CardListComponentBase<DiaryModel>
    {
        private bool ShowDeleteDiary;

        private bool ShowSelectTag;

        private bool ShowExport;

        private DiaryModel SelectedDiary = new();

        private List<DiaryModel> ExportDiaries = new();

        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;

        [Parameter]
        public EventCallback<DiaryModel> OnRemove { get; set; }

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

            var index = _value.FindIndex(it => it.Id == diaryModel.Id);
            if (index < 0)
            {
                return;
            }

            _value.RemoveAt(index);
            await InvokeAsync(StateHasChanged);
            await OnRemove.InvokeAsync(diaryModel);
            if (diaryModel.Private)
            {
                await AlertService.Success(I18n.T("Read.PrivacyAlert"));
            }
        }

        public async Task Export(DiaryModel diary)
        {
            var flag = await PlatformService.TryStorageWritePermission();
            if (!flag)
            {
                return;
            }

            var newDiary = await DiaryService.FindAsync(diary.Id);
            ExportDiaries = new() { newDiary };
            ShowExport = true;
            await InvokeAsync(StateHasChanged);
        }

        protected override void OnInitialized()
        {
            LoadView();
            base.OnInitialized();
        }

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await base.OnInitializedAsync();
        }

        protected override async Task OnResume()
        {
            await LoadSettings();
            await base.OnResume();
        }

        protected override IEnumerable<DiaryModel> Sort(IEnumerable<DiaryModel> value)
        {
            return base.Sort(value).OrderByDescending(it => it.Top);
        }

        private List<TagModel> SelectedTags
        {
            get => SelectedDiary.Tags ?? new();
            set => SelectedDiary.Tags = value;
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
                var index = _value.FindIndex(it => it.Id == diaryModel.Id);
                if (index < 0)
                {
                    return;
                }

                _value.RemoveAt(index);
                await AlertService.Success(I18n.T("Share.DeleteSuccess"));
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                await AlertService.Error(I18n.T("Share.DeleteFail"));
            }
            await OnRemove.InvokeAsync(diaryModel);
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

        private void LoadView()
        {
            SortOptions = new()
            {
                {"Sort.CreateTime.Desc",it => it.OrderByDescending(d => d.CreateTime) },
                {"Sort.CreateTime.Asc",it => it.OrderBy(d => d.CreateTime) },
            };
            SortItem = SortItems.First().Value;
        }
    }
}
