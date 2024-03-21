using BlazorComponent;
using BlazorComponent.JSInterop;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class IndexPage : DiariesPageComponentBase
    {
        private bool showWelcomeText;

        private bool showDate;

        private StringNumber tab = 0;

        private SwiperTabItems swiperTabItems = default!;

        private readonly List<string> tabNames = ["Index.All", "Index.Tag"];

        [Inject]
        private IVersionUpdataManager VersionManager { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            VersionManager.AfterFirstEnter += UpdateDiariesAndStateHasChanged;
            VersionManager.AfterVersionUpdate += UpdateDiariesAndStateHasChanged;
            NavigateService.BeforePopToRoot += BeforePopToRoot;
        }

        protected override void OnDispose()
        {
            VersionManager.AfterFirstEnter -= UpdateDiariesAndStateHasChanged;
            VersionManager.AfterVersionUpdate -= UpdateDiariesAndStateHasChanged;
            NavigateService.BeforePopToRoot -= BeforePopToRoot;
            base.OnDispose();
        }

        protected override void UpdateSettings()
        {
            base.UpdateSettings();

            showWelcomeText = SettingService.Get<bool>(Setting.WelcomeText);
            showDate = SettingService.Get<bool>(Setting.IndexDate);
        }

        private bool ShowAddTag { get; set; }

        private async Task SaveAddTag(string tagName)
        {
            ShowAddTag = false;
            if (string.IsNullOrWhiteSpace(tagName))
            {
                return;
            }

            if (Tags.Any(it => it.Name == tagName))
            {
                await AlertService.Warning(I18n.T("Tag.Repeat.Title"), I18n.T("Tag.Repeat.Content"));
                return;
            }

            TagModel tag = new()
            {
                Name = tagName
            };
            var flag = await TagService.AddAsync(tag);
            if (!flag)
            {
                await AlertService.Error(I18n.T("Share.AddFail"));
                return;
            }

            await AlertService.Success(I18n.T("Share.AddSuccess"));
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

        private Task Search(string? value)
        {
            To($"search?query={value}");
            return Task.CompletedTask;
        }

        private async Task UpdateDiariesAndStateHasChanged()
        {
            await UpdateDiariesAsync();
            await InvokeAsync(StateHasChanged);
        }

        private async Task BeforePopToRoot(PopEventArgs args)
        {
            if (thisPageUrl == args.PreviousUri && thisPageUrl == args.NextUri)
            {
                await JS.ScrollTo(swiperTabItems.ActiveItem.Ref, 0);
            }
        }
    }
}
