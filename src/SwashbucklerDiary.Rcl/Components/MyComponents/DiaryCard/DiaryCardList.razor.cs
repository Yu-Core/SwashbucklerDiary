using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class DiaryCardList : CardListComponentBase<DiaryModel>
    {
        private bool showDeleteDiary;

        private bool showSelectTag;

        private bool showExport;

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

        protected override void OnInitialized()
        {
            base.OnInitialized();

            LoadView();
        }

        protected override IEnumerable<DiaryModel> Sort(IEnumerable<DiaryModel> value)
        {
            return base.Sort(value).OrderByDescending(it => it.Top);
        }

        protected override async Task UpdateSettings()
        {
            var showPrivacyTask = SettingService.Get<bool>(Setting.PrivacyMode);
            var showIconTask = SettingService.Get<bool>(Setting.DiaryCardIcon);
            var dateFormatTask = SettingService.Get<string>(Setting.DiaryCardDateFormat);
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

        private float ItemHeight => MasaBlazor.Breakpoint.Xs ? 156.8f : (MasaBlazor.Breakpoint.Sm ? 164.8f : 172.8f);

        private List<TagModel> SelectedTags
        {
            get => SelectedItemValue.Tags ?? new();
            set => SelectedItemValue.Tags = value;
        }

        private string TopText()
            => SelectedItemValue.Top ? "Diary.CancelTop" : "Diary.Top";

        private string PrivateText()
            => SelectedItemValue.Private ? "Read.ClosePrivacy" : "Read.OpenPrivacy";

        private string PrivateIcon()
            => SelectedItemValue.Private ? "mdi-lock-open-variant-outline" : "mdi-lock-outline";

        private async Task Topping()
        {
            SelectedItemValue.Top = !SelectedItemValue.Top;
            SelectedItemValue.UpdateTime = DateTime.Now;
            await InvokeAsync(StateHasChanged);
            await DiaryService.UpdateAsync(SelectedItemValue);
        }

        private void Delete()
        {
            showDeleteDiary = true;
            InvokeAsync(StateHasChanged);
        }

        private async Task Copy()
        {
            var text = SelectedItemValue.CreateCopyContent();
            await PlatformIntegration.SetClipboard(text);
            await AlertService.Success(I18n.T("Share.CopySuccess"));
        }

        private async Task ChangeTag()
        {
            SelectedTags = await DiaryService.GetTagsAsync(SelectedItemValue.Id);
            await InvokeAsync(StateHasChanged);
            showSelectTag = true;
            await InvokeAsync(StateHasChanged);
        }

        private async Task MovePrivacy()
        {
            SelectedItemValue.Private = !SelectedItemValue.Private;
            SelectedItemValue.UpdateTime = DateTime.Now;
            await DiaryService.UpdateAsync(SelectedItemValue);

            var index = _value.FindIndex(it => it.Id == SelectedItemValue.Id);
            if (index < 0)
            {
                return;
            }

            _value.RemoveAt(index);
            await InvokeAsync(StateHasChanged);
            await OnRemove.InvokeAsync(SelectedItemValue);
            if (SelectedItemValue.Private)
            {
                await AlertService.Success(I18n.T("Read.PrivacyAlert"));
            }
        }

        private async Task Export()
        {
            var newDiary = await DiaryService.FindAsync(SelectedItemValue.Id);
            exportDiaries = [newDiary];
            showExport = true;
            await InvokeAsync(StateHasChanged);
        }

        private async Task ConfirmDelete()
        {
            showDeleteDiary = false;
            bool flag = await DiaryService.DeleteAsync(SelectedItemValue);
            if (flag)
            {
                var index = _value.FindIndex(it => it.Id == SelectedItemValue.Id);
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

            await OnRemove.InvokeAsync(SelectedItemValue);
        }

        private async Task SaveSelectTags()
        {
            showSelectTag = false;
            SelectedItemValue.UpdateTime = DateTime.Now;
            await DiaryService.UpdateTagsAsync(SelectedItemValue);
        }

        private void LoadView()
        {
            sortOptions = new()
            {
                {"Sort.CreateTime.Desc",it => it.OrderByDescending(d => d.CreateTime) },
                {"Sort.CreateTime.Asc",it => it.OrderBy(d => d.CreateTime) },
            };
            sortItem = SortItems.First();
            menuItems = new()
            {
                new(this, "Diary.Tag", "mdi-label-outline", ChangeTag),
                new(this, "Share.Copy", "mdi-content-copy", Copy),
                new(this, "Share.Delete", "mdi-delete-outline", Delete),
                new(this, TopText, "mdi-format-vertical-align-top", Topping),
                new(this, "Diary.Export", "mdi-export", Export),
                new(this, "Share.Sort", "mdi-sort-variant", Sort),
                new(this, PrivateText, PrivateIcon, MovePrivacy, ()=>ShowPrivacy)
            };
        }
    }
}
