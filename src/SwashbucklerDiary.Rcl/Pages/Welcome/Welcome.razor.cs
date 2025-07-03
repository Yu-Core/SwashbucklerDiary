using Microsoft.AspNetCore.Components;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class Welcome
    {
        private bool showLanguga;

        private bool showAgreement;

        [Inject]
        private IDiaryService DiaryService { get; set; } = default!;

        [Inject]
        private IAppLifecycle AppLifecycle { get; set; } = default!;

        [Inject]
        private IThemeService ThemeService { get; set; } = default!;

        protected override void OnInitialized()
        {
            base.OnInitialized();

            UpdateSettings();
            NavigateController.AddHistoryAction(AppLifecycle.QuitApp);
        }

        protected override async ValueTask DisposeAsyncCore()
        {
            await base.DisposeAsyncCore();

            NavigateController.RemoveHistoryAction(AppLifecycle.QuitApp);
        }

        private string BackgroundColor => ThemeService.RealTheme == Theme.Dark ? ThemeColor.DarkSurface : ThemeColor.LightSurface;

        private void UpdateSettings()
        {
            var setLang = SettingService.Get(s => s.FirstSetLanguage);
            var agree = SettingService.Get(s => s.FirstAgree);

            if (setLang && agree)
            {
                NavigateController.RemoveHistoryAction(AppLifecycle.QuitApp);
                NavigationManager.NavigateTo("", replace: true);
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
            I18n.SetCulture(new(value));
            Task[] tasks =
            [
                InsertDefaultDiaries(),
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
            await SettingService.SetAsync(s => s.FirstAgree, true);
            NavigateController.RemoveHistoryAction(AppLifecycle.QuitApp);
            NavigationManager.NavigateTo("", replace: true);
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