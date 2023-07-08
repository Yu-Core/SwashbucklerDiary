using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
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
        private ISystemService SystemService { get; set; } = default!;
        [Inject]
        private II18nService I18n { get; set; } = default!;
        [Inject]
        private IStateService StateService { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadSettings();
        }

        private Dictionary<string, string> Languages => I18n.Languages;

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
                return;

            SelectedLanguage = true;
            I18n.SetCulture(value);
            await InsertDefaultDiaries();
            await SettingsService.Save(SettingType.FirstSetLanguage, true);
            await SettingsService.Save(SettingType.Language, value);
            await StateService.NotifyFirstLauchChanged();
        }

        private async Task OnArgee()
        {
            if (AgreedAgreement)
                return;
            AgreedAgreement = true;
            Show = false;
            await SettingsService.Save(SettingType.FirstAgree, true);
        }
        private void OnDisagree()
        {
            SystemService.QuitApp();
        }

        private async Task InsertDefaultDiaries()
        {
            string[] defaultdiaries = { "FilePath.Functional Description",
                    "FilePath.Diary Meaning", "FilePath.Markdown Syntax" };
            var diaries = await GetDefaultDiaries(defaultdiaries);
            await DiaryService.AddAsync(diaries);
        }

        async Task<List<DiaryModel>> GetDefaultDiaries(string[] keys)
        {
            var diaries = new List<DiaryModel>();
            foreach (string key in keys)
            {
                var content = await SystemService.ReadMarkdownFileAsync(I18n.T(key)!);
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
