using Masa.Blazor;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class HomePage : DiariesPageComponentBase
    {
        private bool showWelcomeText;

        private bool showDate;

        private bool showAddTag;

        private StringNumber tab = 0;

        private SwiperTabItems swiperTabItems = default!;

        private readonly List<TabListItem> tabListItems =
        [
            new("All","all"),
            new("Tag","tag"),
            new("Template","template"),
        ];

        private List<DiaryModel> templates = [];

        protected override void ReadSettings()
        {
            base.ReadSettings();

            showWelcomeText = SettingService.Get(s => s.WelcomeText);
            showDate = SettingService.Get(s => s.IndexDate);
        }

        protected override async Task UpdateDiariesAsync()
        {
            await base.UpdateDiariesAsync();

            templates = await DiaryService.QueryTemplatesAsync();
        }

        private string? SwiperActiveItemSelector
            => swiperTabItems is null || swiperTabItems.ActiveItem is null ? null : $"#{swiperTabItems.ActiveItem.Id}";

        private async Task SaveAddTag(string tagName)
        {
            showAddTag = false;
            if (string.IsNullOrWhiteSpace(tagName))
            {
                return;
            }

            if (Tags.Any(it => it.Name == tagName))
            {
                await AlertService.WarningAsync(I18n.T("Tag already exists"), I18n.T("Do not add again"));
                return;
            }

            TagModel tag = new()
            {
                Name = tagName
            };
            var flag = await TagService.AddAsync(tag);
            if (!flag)
            {
                await AlertService.ErrorAsync(I18n.T("Add failed"));
                return;
            }

            Tags.Insert(0, tag);
            Tags = [.. Tags];
            StateHasChanged();
        }

        private void Search(string? value)
        {
            string? queryParameters = string.IsNullOrWhiteSpace(value) ? null : $"?query={value}";
            To($"search{queryParameters}");
        }
    }
}
