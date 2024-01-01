using BlazorComponent.I18n;
using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Maui.Services
{
    public class I18nService : Rcl.Services.I18nService
    {
        private readonly Lazy<Dictionary<string, string>> _languages;

        public override Dictionary<string, string> Languages => _languages.Value;

        public I18nService(IStaticWebAssets staticWebAssets,
            I18n i18n) : base(i18n)
        {
            _languages = new(() => staticWebAssets.ReadJsonAsync<Dictionary<string, string>>("json/i18n/languages.json").Result);
        }
    }
}
