using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class IndexPage : DiariesPageComponentBase
    {
        private bool showWelcomeText;

        private bool showDate;

        private bool showAddTag;

        private StringNumber tab = 0;

        private SwiperTabItems swiperTabItems = default!;

        private readonly List<TabListItem> tabListItems =
        [
            new("Index.All","all"),
            new("Index.Tag","tag"),
        ];

        [Inject]
        private IVersionUpdataManager VersionManager { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            VersionManager.AfterFirstEnter += UpdateDiariesAndStateHasChanged;
            VersionManager.AfterVersionUpdate += UpdateDiariesAndStateHasChanged;
        }

        protected override void OnDispose()
        {
            VersionManager.AfterFirstEnter -= UpdateDiariesAndStateHasChanged;
            VersionManager.AfterVersionUpdate -= UpdateDiariesAndStateHasChanged;
            base.OnDispose();
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            showWelcomeText = SettingService.Get<bool>(Setting.WelcomeText);
            showDate = SettingService.Get<bool>(Setting.IndexDate);
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
                await PopupServiceHelper.Warning(I18n.T("Tag.Repeat.Title"), I18n.T("Tag.Repeat.Content"));
                return;
            }

            TagModel tag = new()
            {
                Name = tagName
            };
            var flag = await TagService.AddAsync(tag);
            if (!flag)
            {
                await PopupServiceHelper.Error(I18n.T("Share.AddFail"));
                return;
            }

            Tags.Insert(0, tag);
            StateHasChanged();
        }

        private string GetWelcomeText()
        {
            int hour = Convert.ToInt16(DateTime.Now.ToString("HH"));
            if (hour >= 6 && hour < 11)
            {
                return I18n.T("Index.Welcome.Morning")!;
            }
            else if (hour >= 11 && hour < 13)
            {
                return I18n.T("Index.Welcome.Noon")!;
            }
            else if (hour >= 13 && hour < 18)
            {
                return I18n.T("Index.Welcome.Afternoon")!;
            }
            else if (hour >= 18 && hour < 23)
            {
                return I18n.T("Index.Welcome.Night")!;
            }
            else if (hour >= 23 || hour < 6)
            {
                return I18n.T("Index.Welcome.BeforeDawn")!;
            }

            return "Hello World";
        }

        private void Search(string? value)
        {
            string? queryParameters = string.IsNullOrWhiteSpace(value) ? null : $"?query={value}";
            To($"search{queryParameters}");
        }

        private async Task UpdateDiariesAndStateHasChanged()
        {
            await UpdateDiariesAsync();
            await InvokeAsync(StateHasChanged);
        }
    }
}
