using BlazorComponent.I18n;
using SwashbucklerDiary.IServices;
using System.Globalization;

namespace SwashbucklerDiary.Services
{
    public class I18nService : II18nService
    {
        public I18n I18n { get; set; } = default!;
        public CultureInfo Culture => I18n.Culture;

        public void Initialize(I18n i18n)
        {
            I18n = i18n;
        }

        public void SetCulture(string culture)
        {
            I18n.SetCulture(new CultureInfo(culture));
        }

        public string T(string? key)
        {
            if (key == null)
            {
                return string.Empty;
            }

            if(I18n is null)
            {
                return string.Empty;
            }

            return I18n.T(key) ?? key;
        }

        public string ToWeek(DateTime? dateTime = null)
        {
            return T("Week." + ((int)(dateTime ?? DateTime.Now).DayOfWeek).ToString());
        }

    }
}
