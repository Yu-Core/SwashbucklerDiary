using System.Globalization;

namespace SwashbucklerDiary.Rcl.Services
{
    public interface II18nService
    {
        event Action OnChanged;

        CultureInfo Culture { get; }

        Dictionary<string,string> Languages { get; }

        string T(string? key);

        string? T(string? key, bool whenNullReturnKey);

        void SetCulture(string culture);

        string ToWeek(DateTime? dateTime = null);
    }
}
