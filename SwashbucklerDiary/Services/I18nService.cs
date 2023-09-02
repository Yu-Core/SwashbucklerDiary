using BlazorComponent.I18n;
using SwashbucklerDiary.IServices;
using System.Globalization;

namespace SwashbucklerDiary.Services
{
    public class I18nService : II18nService
    {
        private I18n I18n { get; set; } = default!;
        public CultureInfo Culture => I18n.Culture;

        public Dictionary<string, string> Languages { get; } = new()
        {
            {"简体中文","zh-CN" },
            {"English","en-US" }
        };

        public event Action? OnChanged;

        public void Initialize(object i18n)
        {
            I18n = (I18n)i18n;
        }

        public void SetCulture(string culture)
        {
            I18n.SetCulture(new CultureInfo(culture));
            OnChanged?.Invoke();
        }

        public string T(string? key) => T(key, true) ?? key ?? string.Empty;

        public string? T(string? key, bool whenNullReturnKey)
        {
            if (key == null)
            {
                return string.Empty;
            }

            if (I18n is null)
            {
                return string.Empty;
            }

            return I18n.T(key, whenNullReturnKey);
        }

        public string ToWeek(DateTime? dateTime = null)
        {
            return T("Week." + ((int)(dateTime ?? DateTime.Now).DayOfWeek).ToString());
        }

    }
}
