using Masa.Blazor;
using System.Globalization;

namespace SwashbucklerDiary.Rcl.Services
{
    public class I18nService : I18n, II18nService
    {
        public I18nService(MasaBlazorOptions options) : base(options)
        {
            CultureInfo.CurrentCulture.NumberFormat = NumberFormatInfo.InvariantInfo;
            CultureInfo.CurrentUICulture.NumberFormat = NumberFormatInfo.InvariantInfo;
        }

        public new string T(string? key, params object[] args)
            => base.T(key, args) ?? string.Empty;

        public new void SetCulture(CultureInfo culture, CultureInfo? uiCulture = null)
        {
            base.SetCulture(culture, uiCulture);

            CultureInfo.CurrentCulture.NumberFormat = NumberFormatInfo.InvariantInfo;
            CultureInfo.CurrentUICulture.NumberFormat = NumberFormatInfo.InvariantInfo;
        }
    }
}
