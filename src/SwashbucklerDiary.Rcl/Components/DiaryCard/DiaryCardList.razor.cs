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

        private bool showSetPrivacy;

        private bool showIcon;

        private bool showTags;

        private bool privacyMode;

        private List<DiaryModel> exportDiaries = [];

        private readonly DiaryCardListOptions options = new();

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

        protected override DiaryModel SelectedItemValue
        {
            get => options.SelectedItemValue;
            set => options.SelectedItemValue = value;
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

        protected override void ReadSettings()
        {
            base.ReadSettings();

            showSetPrivacy = SettingService.Get<bool>(Setting.SetPrivacyDiary);
            showIcon = SettingService.Get<bool>(Setting.DiaryCardIcon);
            showTags = SettingService.Get<bool>(Setting.DiaryCardTags);
            options.DateFormat = SettingService.Get<string>(Setting.DiaryCardDateFormat);
            var diarySort = SettingService.Get<string>(Setting.DiarySort);
            if (!string.IsNullOrEmpty(diarySort))
            {
                sortItem = diarySort;
            }

            privacyMode = SettingService.GetTemp<bool>(TempSetting.PrivacyMode);
        }

        private float ItemHeight => MasaBlazorHelper.Breakpoint.Xs ? 156.8f : (MasaBlazorHelper.Breakpoint.Sm ? 164.8f : 172.8f);

        private string? InternalClass => $"card-list-main {(showMenu ? "card-list-menu__open " : "")}{(showIcon ? "" : "card-list-icon__none ")}{(showTags ? "" : "card-list-tags__none")}";

        private List<TagModel> SelectedTags
        {
            get => SelectedItemValue.Tags ?? [];
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
            await DiaryService.UpdateAsync(SelectedItemValue, it => new { it.Top, it.UpdateTime });
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
            await PopupServiceHelper.Success(I18n.T("Share.CopySuccess"));
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
            await DiaryService.UpdateAsync(SelectedItemValue, it => new { it.Private, it.UpdateTime });

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
                await PopupServiceHelper.Success(I18n.T("Read.PrivacyAlert"));
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
                await PopupServiceHelper.Success(I18n.T("Share.DeleteSuccess"));
                await InvokeAsync(StateHasChanged);
            }
            else
            {
                await PopupServiceHelper.Error(I18n.T("Share.DeleteFail"));
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
                {"Sort.Time.Desc",it => it.OrderByDescending(d => d.CreateTime) },
                {"Sort.Time.Asc",it => it.OrderBy(d => d.CreateTime) },
            };

            if (string.IsNullOrEmpty(sortItem))
            {
                sortItem = SortItems.First();
            }

            menuItems =
            [
                new(this, "Diary.Tag", "mdi-label-outline", ChangeTag),
                new(this, "Share.Copy", "mdi-content-copy", Copy),
                new(this, "Share.Delete", "mdi-delete-outline", Delete),
                new(this, TopText, "mdi-format-vertical-align-top", Topping),
                new(this, "Diary.Export", "mdi-export", Export),
                new(this, "Share.Sort", "mdi-sort-variant", Sort),
                new(this, "Read.CopyReference", "mdi-link-variant", CopyReference),
                new(this, PrivateText, PrivateIcon, MovePrivacy, ()=>privacyMode || showSetPrivacy)
            ];
        }

        private async Task SortChanged(string value)
        {
            await SettingService.Set(Setting.DiarySort, value);
        }

        private async Task CopyReference()
        {
            var text = $"[{I18n.T("Read.DiaryLink")}](read/{SelectedItemValue.Id})";
            await PlatformIntegration.SetClipboard(text);
            await PopupServiceHelper.Success(I18n.T("Share.CopySuccess"));
        }
    }
}
