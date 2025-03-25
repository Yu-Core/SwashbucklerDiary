using System.Globalization;

namespace SwashbucklerDiary.Rcl.Services
{
    public interface II18nService
    {
        event Action<CultureInfo> OnChanged;

        CultureInfo Culture { get; }

        IEnumerable<CultureInfo> SupportedCultures { get; }

        string T(string? key);

        string? T(string? key, bool whenNullReturnKey);

        void SetCulture(string culture);

        string ToWeek(DateTime? dateTime = null);
    }
}
