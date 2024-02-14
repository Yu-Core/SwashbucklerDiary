using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class DiaryCardList : CardListComponentBase<DiaryModel>
    {
        private bool showDeleteDiary;

        private bool showSelectTag;

        private bool showExport;

        private DiaryModel selectedDiary = new();

        private List<DiaryModel> exportDiaries = [];

        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;

        [Parameter]
        public EventCallback<DiaryModel> OnRemove { get; set; }

        [Parameter]
        public List<TagModel> Tags { get; set; } = [];

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
            selectedDiary = diaryModel;
            showDeleteDiary = true;
            InvokeAsync(StateHasChanged);
        }

        public async Task Copy(DiaryModel diaryModel)
        {
            var text = DiaryCopyContent(diaryModel);
            await PlatformIntegration.SetClipboard(text);
            await AlertService.Success(I18n.T("Share.CopySuccess"));
        }

        public async Task ChangeTag(DiaryModel diary)
        {
            selectedDiary = diary;
            SelectedTags = await DiaryService.GetTagsAsync(selectedDiary.Id);
            await InvokeAsync(StateHasChanged);
            showSelectTag = true;
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
            var newDiary = await DiaryService.FindAsync(diary.Id);
            exportDiaries = [newDiary];
            showExport = true;
            await InvokeAsync(StateHasChanged);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LoadView();
        }

        protected override IEnumerable<DiaryModel> Sort(IEnumerable<DiaryModel> value)
        {
            return base.Sort(value).OrderByDescending(it => it.Top);
        }

        private List<TagModel> SelectedTags
        {
            get => selectedDiary.Tags ?? new();
            set => selectedDiary.Tags = value;
        }

        protected override async Task UpdateSettings()
        {
            var showPrivacyTask = SettingService.Get<bool>(Setting.PrivacyMode);
            var showIconTask =  SettingService.Get<bool>(Setting.DiaryCardIcon);
            var  dateFormatTask =  SettingService.Get<string>(Setting.DiaryCardDateFormat);
            Task[] tasks = [
                base.UpdateSettings(),
                showPrivacyTask, 
                showIconTask, 
                dateFormatTask,
            ];
            await Task.WhenAll(tasks);
            ShowPrivacy = showPrivacyTask.Result;
            ShowIcon = showIconTask.Result;
            DateFormat = dateFormatTask.Result;
        }

        private async Task ConfirmDelete()
        {
            var diaryModel = selectedDiary;
            showDeleteDiary = false;
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
            showSelectTag = false;
            selectedDiary.UpdateTime = DateTime.Now;
            await DiaryService.UpdateTagsAsync(selectedDiary);
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
            sortOptions = new()
            {
                {"Sort.CreateTime.Desc",it => it.OrderByDescending(d => d.CreateTime) },
                {"Sort.CreateTime.Asc",it => it.OrderBy(d => d.CreateTime) },
            };
            sortItem = SortItems.First();
        }
    }
}
