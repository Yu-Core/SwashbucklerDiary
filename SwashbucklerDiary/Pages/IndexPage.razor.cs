using BlazorComponent;
using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Pages
{
    public partial class IndexPage : DiariesPageComponentBase
    {
        private bool ShowWelcomeText;

        private bool ShowDate;

        private StringNumber tab = 0;

        [Inject]
        private IStateService StateService { get; set; } = default!;

        public async Task LoadSettings()
        {
            ShowWelcomeText = await SettingsService.Get(SettingType.WelcomeText);
            ShowDate = await SettingsService.Get(SettingType.Date);
        }

        protected override void OnInitialized()
        {
            FirstLauch();
            base.OnInitialized();
        }

        protected override async Task OnParametersSetAsync()
        {
            await LoadSettings();
            await base.OnParametersSetAsync();
        }

        protected override void OnDispose()
        {
            StateService.FirstLauch -= FirstLauchUpdateDiaries;
            base.OnDispose();
        }

        protected override async Task OnResume()
        {
            await LoadSettings();
            await base.OnResume();
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

        private void FirstLauch()
        {
            StateService.FirstLauch += FirstLauchUpdateDiaries;
        }

        private async Task FirstLauchUpdateDiaries()
        {
            await UpdateDiariesAsync();
            await InvokeAsync(StateHasChanged);
        }
    }
}
