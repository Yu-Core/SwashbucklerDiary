using Masa.Blazor;
using System.Globalization;

namespace SwashbucklerDiary.Rcl.Services
{
    public class I18nService : II18nService
    {
        private readonly I18n _i18n;

        public CultureInfo Culture => _i18n.Culture;

        public IEnumerable<CultureInfo> SupportedCultures => _i18n.SupportedCultures;

        public event Action<CultureInfo>? OnChanged;

        public I18nService(I18n i18n)
        {
            _i18n = i18n;
        }

        public void SetCulture(string culture)
        {
            _i18n.SetCulture(new CultureInfo(culture));
            OnChanged?.Invoke(Culture);
        }

        public string T(string? key) => T(key, true) ?? key ?? string.Empty;

        public string? T(string? key, bool whenNullReturnKey)
        {
            if (key == null)
            {
                return string.Empty;
            }

            if (_i18n is null)
            {
                return string.Empty;
            }

            return _i18n.T(key, whenNullReturnKey);
        }

        public string ToWeek(DateTime? dateTime = null)
        {
            return T((dateTime ?? DateTime.Now).DayOfWeek.ToString());
        }
    }
}
