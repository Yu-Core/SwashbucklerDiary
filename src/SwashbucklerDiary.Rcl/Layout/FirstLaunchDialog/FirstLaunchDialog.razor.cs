using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Layout
{
    public partial class FirstLaunchDialog
    {
        private bool show = true;

        private bool showLanguga;

        private bool showAgreement;

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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);

            if (firstRender)
            {
                await UpdateSettings();
                StateHasChanged();
            }
        }

        private async Task UpdateSettings()
        {
            var langTask = Preferences.Get<bool>(Setting.FirstSetLanguage);
            var agreeTask = Preferences.Get<bool>(Setting.FirstAgree);
            await Task.WhenAll(langTask, agreeTask);

            var lang = langTask.Result;
            var agree = agreeTask.Result;

            if (lang && agree)
            {
                show = false;
            }
            else
            {
                showLanguga = !lang;
                showAgreement = !agree;
            }
        }

        private async Task SetLanguage(string value)
        {
            if (!showLanguga)
            {
                return;
            }

            showLanguga = false;
            I18n.SetCulture(value);
            Task insertTask = Task.Run(async () =>
            {
                await InsertDefaultDiaries();
                await VersionManager.FirstEnter();
            });
            Task[] tasks =
            [
                insertTask,
                Preferences.Set(Setting.FirstSetLanguage, true),
                Preferences.Set(Setting.Language, value)
            ];
            await Task.WhenAll(tasks);
        }

        private async Task Argee()
        {
            if (!showAgreement)
            {
                return;
            }

            showAgreement = false;
            show = false;
            await Preferences.Set(Setting.FirstAgree, true);
        }

        private void Disagree()
        {
            AppLifecycle.QuitApp();
        }

        private async Task InsertDefaultDiaries()
        {
            string[] defaultdiaries = { "FilePath.Functional Description", "FilePath.Diary Meaning", "FilePath.Markdown Syntax" };
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
