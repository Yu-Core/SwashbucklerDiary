using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Shared
{
    public partial class FirstLaunchDialog
    {
        private bool Show = true;

        private bool SelectedLanguage;

        private bool AgreedAgreement;

        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;

        [Inject]
        private ISettingsService SettingsService { get; set; } = default!;

        [Inject]
        private IPlatformService PlatformService { get; set; } = default!;

        [Inject]
        private II18nService I18n { get; set; } = default!;

        [Inject]
        private IStateService StateService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await base.OnInitializedAsync();
        }

        private bool ShowLanguga => !SelectedLanguage;

        private bool ShowAgreement => SelectedLanguage;

        private async Task LoadSettings()
        {
            var lang = await SettingsService.Get<bool>(SettingType.FirstSetLanguage);
            var agree = await SettingsService.Get<bool>(SettingType.FirstAgree);
            
            if (lang && agree)
            {
                Show = false;
            }
            else
            {
                SelectedLanguage = lang;
                AgreedAgreement = agree;
            }
        }

        private async Task SetLanguage(string value)
        {
            if (SelectedLanguage)
            {
                return;
            }

            SelectedLanguage = true;
            I18n.SetCulture(value);
            await InsertDefaultDiaries();
            await SettingsService.Save(SettingType.FirstSetLanguage, true);
            await SettingsService.Save(SettingType.Language, value);
            await StateService.NotifyFirstLauchChanged();
        }

        private async Task Argee()
        {
            if (AgreedAgreement)
            {
                return;
            }

            AgreedAgreement = true;
            Show = false;
            await SettingsService.Save(SettingType.FirstAgree, true);
        }

        private void Disagree()
        {
            PlatformService.QuitApp();
        }

        private async Task InsertDefaultDiaries()
        {
            string[] defaultdiaries = { "FilePath.Functional Description","FilePath.Diary Meaning", "FilePath.Markdown Syntax" };
            var diaries = await GetDefaultDiaries(defaultdiaries);
            await DiaryService.AddAsync(diaries);
        }

        private async Task<List<DiaryModel>> GetDefaultDiaries(string[] keys)
        {
            var diaries = new List<DiaryModel>();
            foreach (string key in keys)
            {
                var content = await PlatformService.ReadMarkdownFileAsync(I18n.T(key)!);
                var diary = new DiaryModel()
                {
                    Content = content,
                };
                diaries.Add(diary);
            }
            return diaries;
        }
    }
}
