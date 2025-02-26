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

        [CascadingParameter(Name = "IsDark")]
        public bool Dark { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();

            UpdateSettings();
        }

        private string BackgroundColor => Dark ? ThemeColor.DarkSurface : ThemeColor.LightSurface;

        private void UpdateSettings()
        {
            var setLang = SettingService.Get(s => s.FirstSetLanguage);
            var agree = SettingService.Get(s => s.FirstAgree);

            if (setLang && agree)
            {
                show = false;
                VersionManager.NotifyAfterCheckFirstLaunch();
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
                VersionManager.NotifyAfterFirstEnter();
            });
            Task[] tasks =
            [
                insertTask,
                SettingService.SetAsync(s => s.FirstSetLanguage, true),
                SettingService.SetAsync(s => s.Language, value)
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
            await SettingService.SetAsync(s => s.FirstAgree, true);
            VersionManager.NotifyAfterCheckFirstLaunch();
        }

        private void Disagree()
        {
            AppLifecycle.QuitApp();
        }

        private async Task InsertDefaultDiaries()
        {
            string[] initWriteDocPaths = await StaticWebAssets.ReadJsonAsync<string[]>("json/Init-write-doc/doc-path.json");
            var diaries = await GetDefaultDiaries(initWriteDocPaths);
            await DiaryService.AddAsync(diaries);
        }

        private async Task<List<DiaryModel>> GetDefaultDiaries(string[] paths)
        {
            var diaries = new List<DiaryModel>();
            foreach (string path in paths)
            {
                var uri = $"{path}{I18n.Culture}.md";
                var content = await StaticWebAssets.ReadContentAsync(uri);
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
