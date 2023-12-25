using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Layout
{
    public partial class FirstLaunchDialog
    {
        private bool Show = true;

        private bool SelectedLanguage;

        private bool AgreedAgreement;

        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;

        [Inject]
        private IPreferences Preferences { get; set; } = default!;

        [Inject]
        private IAppLifecycle AppLifecycle { get; set; } = default!;

        [Inject]
        private II18nService I18n { get; set; } = default!;

        [Inject]
        private IVersionUpdataManager VersionManager { get; set; } = default!;

        [Inject]
        private IStaticWebAssets StaticWebAssets { get; set; } = default!;

        protected override async Task OnInitializedAsync()
        {
            await LoadSettings();
            await base.OnInitializedAsync();
        }

        private bool ShowLanguga => !SelectedLanguage;

        private bool ShowAgreement => SelectedLanguage;

        private async Task LoadSettings()
        {
            var lang = await Preferences.Get<bool>(Setting.FirstSetLanguage);
            var agree = await Preferences.Get<bool>(Setting.FirstAgree);
            
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
            await Preferences.Set(Setting.FirstSetLanguage, true);
            await Preferences.Set(Setting.Language, value);
            await VersionManager.FirstEnter();
        }

        private async Task Argee()
        {
            if (AgreedAgreement)
            {
                return;
            }

            AgreedAgreement = true;
            Show = false;
            await Preferences.Set(Setting.FirstAgree, true);
        }

        private void Disagree()
        {
            AppLifecycle.QuitApp();
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
                var content = await StaticWebAssets.ReadContentAsync(I18n.T(key)!);
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
