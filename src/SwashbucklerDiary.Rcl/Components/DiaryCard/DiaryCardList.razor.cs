using Masa.Blazor.Core;
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

        private bool showLocation;

        private bool privacyMode;

        private string? urlScheme;

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

        [Parameter]
        public bool Template
        {
            get => options.Template;
            set => options.Template = value;
        }

        [Parameter]
        public EventCallback<DiaryModel> OnClick { get; set; }

        public void UpdateSettings()
            => ReadSettings();

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
            var diaries = base.Sort(value).OrderByDescending(it => it.Top);
            if (Template)
            {
                diaries = diaries.OrderByDescending(it => it.Template && options.DefaultTemplateId == it.Id);
            }

            return diaries;
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            showSetPrivacy = SettingService.Get(s => s.SetPrivacyDiary);
            showIcon = SettingService.Get(s => s.DiaryCardIcon);
            showTags = SettingService.Get(s => s.DiaryCardTags);
            showLocation = SettingService.Get(s => s.DiaryCardLocation);
            options.TimeFormat = SettingService.Get(s => s.DiaryCardTimeFormat);
            urlScheme = SettingService.Get(s => s.UrlScheme);
            options.Markdown = SettingService.Get(s => s.DiaryCardMarkdown);
            string defaultTemplateIdString = SettingService.Get(s => s.DefaultTemplateId);
            if (Guid.TryParse(defaultTemplateIdString, out var defaultTemplateId))
            {
                options.DefaultTemplateId = defaultTemplateId;
            }

            var diarySort = SettingService.Get(s => s.DiarySort);
            if (!string.IsNullOrEmpty(diarySort))
            {
                SortItem = diarySort;
            }

            privacyMode = SettingService.GetTemp(s => s.PrivacyMode);
        }

        private string? InternalClass => new CssBuilder()
            .Add("card-list__main")
            .Add("card-list__menu--active", showMenu)
            .Add("diary-card-list__icon--hidden", !showIcon)
            .Add("diary-card-list__tags--hidden", !showTags)
            .Add("diary-card-list__location--hidden", !showLocation)
            .ToString();

        private List<TagModel> SelectedTags
        {
            get => SelectedItem.Tags ?? [];
            set => SelectedItem.Tags = value;
        }

        private string TopText() => SelectedItem.Top ? "Cancel top" : "Top";

        private string DefaultTemplateText() => options.DefaultTemplateId != SelectedItem.Id ? "Set as default template" : "Cancel default template";

        private string PrivateText() => privacyMode ? "Cancel privacy" : "Set to private";

        private string PrivateIcon() => privacyMode ? "lock_open" : "lock";

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
            await PlatformIntegration.SetClipboardAsync(text);
            await AlertService.SuccessAsync(I18n.T("Copy successfully"));
        }

        private async Task OpenTagDialog()
        {
            SelectedTags = await DiaryService.GetTagsAsync(SelectedItem.Id);
            showSelectTag = true;
            await InvokeAsync(StateHasChanged);
        }

        private async Task MovePrivacy()
        {
            await DiaryService.MovePrivacyDiaryAsync(SelectedItem, !privacyMode);
            RemoveSelectedItem();
            await InvokeAsync(StateHasChanged);
            if (privacyMode)
            {
                await AlertService.SuccessAsync(I18n.T("Removed from privacy mode"));
            }
            else
            {
                await AlertService.SuccessAsync(I18n.T("Moved to privacy mode"));
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
                    await AlertService.SuccessAsync(I18n.T("Delete successfully"));
                    await InvokeAsync(StateHasChanged);
                }
            }
            else
            {
                await AlertService.ErrorAsync(I18n.T("Delete failed"));
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
                {"Time - Reverse order",it => it.OrderByDescending(d => d.CreateTime) },
                {"Time - Positive order",it => it.OrderBy(d => d.CreateTime) },
                {"Update Time - Reverse order",it => it.OrderByDescending(d => d.UpdateTime) },
                {"Update Time - Positive order",it => it.OrderBy(d => d.UpdateTime) },
            };

            if (string.IsNullOrEmpty(SortItem))
            {
                SortItem = SortItems.First();
            }

            menuItems =
            [
                new(this, "Tag", "label", OpenTagDialog),
                new(this, "Copy", "content_copy", Copy),
                new(this, "Delete", "mdi:mdi-delete-outline", Delete),
                new(this, TopText, "vertical_align_top", Topping),
                new(this, "Export", "mdi:mdi-export", Export),
                new(this, "Sort", "sort", OpenSortDialog),
                new(this, "Copy reference", "format_quote", CopyReference),
                new(this, "Copy external link", "mdi:mdi-link-variant", CopyExternalLink),
                new(this, DefaultTemplateText, "space_dashboard", SetDefaultTemplateAsync, ()=>SelectedItem.Template),
                new(this, PrivateText, PrivateIcon, MovePrivacy, ()=>privacyMode || showSetPrivacy)
            ];
        }

        private async Task SortChanged(string value)
        {
            await SettingService.SetAsync(s => s.DiarySort, value);
        }

        private async Task CopyReference()
        {
            var text = SelectedItem.GetReferenceText(I18n);
            await PlatformIntegration.SetClipboardAsync(text);
            await AlertService.SuccessAsync(I18n.T("Copy successfully"));
        }

        private async Task CopyExternalLink()
        {
            string text;
            if (PlatformIntegration.CurrentPlatform == AppDevicePlatform.Browser)
            {
                text = NavigationManager.ToAbsoluteUri($"read/{SelectedItem.Id}").ToString();
            }
            else
            {
                text = $"{urlScheme}://read/{SelectedItem.Id}";
            }

            await PlatformIntegration.SetClipboardAsync(text);
            await AlertService.SuccessAsync(I18n.T("Copy successfully"));
        }

        private async void ClickCard(DiaryModel diary)
        {
            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync(diary);
            }
            else
            {
                To($"read/{diary.Id}");
            }
        }

        private async Task SetDefaultTemplateAsync()
        {
            if (options.DefaultTemplateId == SelectedItem.Id)
            {
                options.DefaultTemplateId = null;
                await SettingService.RemoveAsync(it => it.DefaultTemplateId);
            }
            else
            {
                options.DefaultTemplateId = SelectedItem.Id;
                await SettingService.SetAsync(it => it.DefaultTemplateId, options.DefaultTemplateId.ToString());
            }

            if (Template)
            {
                UpdateInternalValue();
            }
        }
    }
}
