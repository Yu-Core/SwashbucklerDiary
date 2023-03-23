using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.Components
{
    public partial class FirstLaunchDialog : DialogComponentBase
    {
        private bool FirstLaunchLanguage;
        private bool FirstLaunchReadAgreementAndPolicy;

        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;
        [Inject]
        private ISettingsService SettingsService { get; set; } = default!;
        [Inject]
        private ISystemService SystemService { get; set; } = default!;

        [Parameter]
        public EventCallback Agreed { get; set; }

        protected override void OnInitialized()
        {
            LoadSettings();
            base.OnInitialized();
        }

        private Dictionary<string, string> Languages => I18n.Languages;

        private async void LoadSettings()
        {
            FirstLaunchLanguage = SettingsService.GetDefault<bool>(SettingType.FirstSetLanguage);
            FirstLaunchReadAgreementAndPolicy = SettingsService.GetDefault<bool>(SettingType.FirstAgree);
            if(FirstLaunchLanguage)
            {
                if(FirstLaunchReadAgreementAndPolicy)
                {
                    await InternalValueChanged(false);
                }
            }
        }

        private async Task SetLanguage(string value)
        {
            if (FirstLaunchLanguage)
                return;

            FirstLaunchLanguage = true;
            I18n.SetCulture(value);
            await InsertDefaultDiaries();
            await SettingsService.Save(SettingType.FirstSetLanguage, true);
            await SettingsService.Save(SettingType.Language, value);
        }

        private async Task OnArgee()
        {
            await SettingsService.Save(SettingType.FirstAgree, true);
            if (Agreed.HasDelegate)
            {
                await Agreed.InvokeAsync();
            }

            await InternalValueChanged(false);
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
            List<DiaryModel> diaries = new List<DiaryModel>();
            foreach (string key in keys)
            {
                var content = await SystemService.ReadMarkdownFile(I18n.T(key)!);
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
