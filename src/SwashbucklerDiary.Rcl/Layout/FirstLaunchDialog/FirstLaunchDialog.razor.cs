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
        private ISettingService SettingService { get; set; } = default!;

        [Inject]
        private IAppLifecycle AppLifecycle { get; set; } = default!;

        [Inject]
        private II18nService I18n { get; set; } = default!;

        [Inject]
        private IVersionUpdataManager VersionManager { get; set; } = default!;

        [Inject]
        private IStaticWebAssets StaticWebAssets { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            UpdateSettings();
        }

        private async void UpdateSettings()
        {
            var setLang = SettingService.Get<bool>(Setting.FirstSetLanguage);
            var agree = SettingService.Get<bool>(Setting.FirstAgree);

            if (setLang && agree)
            {
                show = false;
                await VersionManager.NotifyAfterCheckFirstLaunch();
            }
            else
            {
                //如果没选择语言就显示语言弹窗，没同意隐私政策就显示隐私政策弹窗
                showLanguga = !setLang;
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
                await VersionManager.NotifyAfterFirstEnter();
            });
            Task[] tasks =
            [
                insertTask,
                SettingService.Set(Setting.FirstSetLanguage, true),
                SettingService.Set(Setting.Language, value)
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
            await SettingService.Set(Setting.FirstAgree, true);
            await VersionManager.NotifyAfterCheckFirstLaunch();
        }

        private void Disagree()
        {
            AppLifecycle.QuitApp();
        }

        private async Task InsertDefaultDiaries()
        {
            string[] defaultdiaries = ["FilePath.Functional Description", "FilePath.Diary Meaning", "FilePath.Markdown Syntax"];
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
