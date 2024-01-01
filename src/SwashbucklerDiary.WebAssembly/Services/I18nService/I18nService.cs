using BlazorComponent.I18n;

namespace SwashbucklerDiary.WebAssembly.Services
{
    public class I18nService : Rcl.Services.I18nService
    {
        private readonly Dictionary<string, string> _languages;

        public override Dictionary<string, string> Languages => _languages;
        public I18nService([FromKeyedServices(nameof(Languages))] Dictionary<string, string> languages,
            I18n i18n) : base(i18n)
        {
            _languages = languages;
        }
    }
}
