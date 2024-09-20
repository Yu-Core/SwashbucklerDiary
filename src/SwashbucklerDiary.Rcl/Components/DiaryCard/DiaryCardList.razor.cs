using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Components
{
    public partial class DiaryCardList : CardListComponentBase<DiaryModel>
    {
        private bool showDelete;

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
        public List<TagModel> Tags { get; set; } = [];

        [Parameter]
        public EventCallback<List<TagModel>> TagsChanged { get; set; }

        [Parameter]
        public string? NotFoundText { get; set; }

        protected override DiaryModel SelectedItem
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

            showSetPrivacy = SettingService.Get(s => s.SetPrivacyDiary);
            showIcon = SettingService.Get(s => s.DiaryCardIcon);
            showTags = SettingService.Get(s => s.DiaryCardTags);
            options.TimeFormat = SettingService.Get(s => s.DiaryCardTimeFormat);
            var diarySort = SettingService.Get(s => s.DiarySort);
            if (!string.IsNullOrEmpty(diarySort))
            {
                SortItem = diarySort;
            }

            privacyMode = SettingService.GetTemp(s => s.PrivacyMode);
        }

        private float ItemHeight => MasaBlazorHelper.Breakpoint.Xs ? 156.8f : (MasaBlazorHelper.Breakpoint.Sm ? 164.8f : 172.8f);

        private string? InternalClass => $"card-list__main {(showMenu ? "card-list__menu--active " : "")}{(showIcon ? "" : "diary-card-list__icon--hidden ")}{(showTags ? "" : "diary-card-list__tags--hidden")}";

        private List<TagModel> SelectedTags
        {
            get => SelectedItem.Tags ?? [];
            set => SelectedItem.Tags = value;
        }

        private string TopText()
            => SelectedItem.Top ? "Diary.CancelTop" : "Diary.Top";

        private string PrivateText()
            => SelectedItem.Private ? "Read.ClosePrivacy" : "Read.OpenPrivacy";

        private string PrivateIcon()
            => SelectedItem.Private ? "mdi-lock-open-variant-outline" : "mdi-lock-outline";

        private async Task Topping()
        {
            SelectedItem.Top = !SelectedItem.Top;
            SelectedItem.UpdateTime = DateTime.Now;
            NotifyValueChanged();
            await InvokeAsync(StateHasChanged);
            await DiaryService.UpdateAsync(SelectedItem, it => new { it.Top, it.UpdateTime });
        }

        private void Delete()
        {
            showDelete = true;
            InvokeAsync(StateHasChanged);
        }

        private async Task Copy()
        {
            var text = SelectedItem.CreateCopyContent();
            await PlatformIntegration.SetClipboard(text);
            await PopupServiceHelper.Success(I18n.T("Share.CopySuccess"));
        }

        private async Task OpenTagDialog()
        {
            SelectedTags = await DiaryService.GetTagsAsync(SelectedItem.Id);
            showSelectTag = true;
            await InvokeAsync(StateHasChanged);
        }

        private async Task MovePrivacy()
        {
            SelectedItem.Private = !SelectedItem.Private;
            SelectedItem.UpdateTime = DateTime.Now;
            await DiaryService.UpdateAsync(SelectedItem, it => new { it.Private, it.UpdateTime });
            RemoveSelectedItem();
            await InvokeAsync(StateHasChanged);
            if (SelectedItem.Private)
            {
                await PopupServiceHelper.Success(I18n.T("Read.PrivacyAlert"));
            }
        }

        private async Task Export()
        {
            var newDiary = await DiaryService.FindAsync(SelectedItem.Id);
            exportDiaries = [newDiary];
            showExport = true;
            await InvokeAsync(StateHasChanged);
        }

        private async Task ConfirmDelete()
        {
            showDelete = false;
            bool flag = await DiaryService.DeleteAsync(SelectedItem);
            if (flag)
            {
                if (RemoveSelectedItem())
                {
                    await PopupServiceHelper.Success(I18n.T("Share.DeleteSuccess"));
                    await InvokeAsync(StateHasChanged);
                }
            }
            else
            {
                await PopupServiceHelper.Error(I18n.T("Share.DeleteFail"));
            }
        }

        private async Task SaveSelectTags()
        {
            SelectedItem.UpdateTime = DateTime.Now;
            await DiaryService.UpdateTagsAsync(SelectedItem);
            NotifyValueChanged();
        }

        private void LoadView()
        {
            sortOptions = new()
            {
                {"Sort.Time.Desc",it => it.OrderByDescending(d => d.CreateTime) },
                {"Sort.Time.Asc",it => it.OrderBy(d => d.CreateTime) },
            };

            if (string.IsNullOrEmpty(SortItem))
            {
                SortItem = SortItems.First();
            }

            menuItems =
            [
                new(this, "Diary.Tag", "mdi-label-outline", OpenTagDialog),
                new(this, "Share.Copy", "mdi-content-copy", Copy),
                new(this, "Share.Delete", "mdi-delete-outline", Delete),
                new(this, TopText, "mdi-format-vertical-align-top", Topping),
                new(this, "Diary.Export", "mdi-export", Export),
                new(this, "Share.Sort", "mdi-sort-variant", OpenSortDialog),
                new(this, "Read.CopyReference", "mdi-link-variant", CopyReference),
                new(this, PrivateText, PrivateIcon, MovePrivacy, ()=>privacyMode || showSetPrivacy)
            ];
        }

        private async Task SortChanged(string value)
        {
            await SettingService.SetAsync(s => s.DiarySort, value);
        }

        private async Task CopyReference()
        {
            var text = $"[{I18n.T("Read.DiaryLink")}](read/{SelectedItem.Id})";
            await PlatformIntegration.SetClipboard(text);
            await PopupServiceHelper.Success(I18n.T("Share.CopySuccess"));
        }
    }
}
