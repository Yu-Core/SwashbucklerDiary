using Masa.Blazor;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Extensions;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Rcl.Services;
using System.Globalization;

namespace SwashbucklerDiary.Rcl.Layout
{
    public abstract partial class MainLayoutBase : LayoutComponentBase, IDisposable
    {
        protected bool afterInitSetting;

        protected readonly List<NavigationButton> navigationButtons = [
            new("Main.Diary", "mdi-notebook-outline", "mdi-notebook", ""),
            new("Main.History", "mdi-clock-outline", "mdi-clock", "history"),
            new("Main.FileBrowse", "mdi-file-outline", "mdi-file", "fileBrowse"),
            new("Main.Mine",  "mdi-account-outline", "mdi-account", "mine"),
        ];

        protected IEnumerable<string> permanentPaths = [];

        [Inject]
        protected NavigationManager NavigationManager { get; set; } = default!;

        [Inject]
        protected INavigateController NavigateController { get; set; } = default!;

        [Inject]
        protected II18nService I18n { get; set; } = default!;

        [Inject]
        protected ISettingService SettingService { get; set; } = default!;

        [Inject]
        protected IVersionUpdataManager VersionUpdataManager { get; set; } = default!;

        [Inject]
        protected MasaBlazor MasaBlazor { get; set; } = default!;

        [Inject]
        protected IJSRuntime JSRuntime { get; set; } = default!;

        [Inject]
        protected IGlobalConfiguration GlobalConfiguration { get; set; } = default!;

        public void Dispose()
        {
            OnDispose();
            GC.SuppressFinalize(this);
        }

        protected bool IsPermanentPath
            => permanentPaths.Any(it => it == NavigationManager.GetAbsolutePath());

        protected override void OnInitialized()
        {
            base.OnInitialized();
            I18n.OnChanged += LanguageChanged;
            permanentPaths = navigationButtons.Select(it => NavigationManager.ToAbsoluteUri(it.Href).AbsolutePath).ToList();
            InitNavigateController();
        }

        protected virtual void OnDispose()
        {
            I18n.OnChanged -= LanguageChanged;
        }

        protected virtual async Task InitSettingsAsync()
        {
            await SettingService.InitializeAsync();
            await GlobalConfiguration.InitializeAsync();
            afterInitSetting = true;
        }

        protected async void LanguageChanged(CultureInfo cultureInfo)
        {
            await InvokeAsync(StateHasChanged);
            await JSRuntime.EvaluateJavascript($"document.documentElement.lang = '{cultureInfo.Name}';");
        }

        protected void InitNavigateController()
        {
            NavigateController.Init(NavigationManager, JSRuntime, permanentPaths);
        }
    }
}
