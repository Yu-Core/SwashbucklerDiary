using Masa.Blazor;
using SwashbucklerDiary.Rcl.Components;
using SwashbucklerDiary.Rcl.Models;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Pages
{
    public partial class UrlSchemePage : ImportantComponentBase
    {
        private List<UrlSchemePath> urlSchemePaths = [];

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();

            await ReadJson();
        }

        protected override void ReadSettings()
        {
            base.ReadSettings();

            UrlScheme = SettingService.Get(s => s.UrlScheme);
        }

        protected override void RegisterWatchers(PropertyWatcher watcher)
        {
            base.RegisterWatchers(watcher);

            watcher.Watch<StringNumber>(nameof(UrlScheme), async () =>
            {
                await SettingService.SetAsync(s => s.UrlScheme, UrlScheme.ToString());
            });
        }

        private StringNumber UrlScheme
        {
            get => GetValue<StringNumber>() ?? SchemeConstants.SwashbucklerDiary;
            set => SetValue(value);
        }

        private async Task ReadJson()
        {
            urlSchemePaths = await StaticWebAssets.ReadJsonAsync<List<UrlSchemePath>>("json/url-scheme/url-scheme-paths.json");
        }

        private async Task CopyAsync(string path)
        {
            var text = $"{UrlScheme}://{path}";
            await PlatformIntegration.SetClipboardAsync(text);
            await PopupServiceHelper.Success(I18n.T("Share.CopySuccess"));
        }
    }
}