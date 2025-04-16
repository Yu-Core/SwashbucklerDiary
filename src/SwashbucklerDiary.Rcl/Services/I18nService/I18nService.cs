using Masa.Blazor;

namespace SwashbucklerDiary.Rcl.Services
{
    public class I18nService : I18n, II18nService
    {
        public I18nService(MasaBlazorOptions options) : base(options)
        {
        }

        public new string T(string? key, params object[] args)
            => base.T(key, args) ?? string.Empty;
    }
}
