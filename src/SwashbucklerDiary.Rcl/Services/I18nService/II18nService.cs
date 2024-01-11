using System.Globalization;

namespace SwashbucklerDiary.Rcl.Services
{
    public interface II18nService
    {
        bool Initialized { get; protected set; }

        event Action OnChanged;

        CultureInfo Culture { get; }

        Dictionary<string,string> Languages { get; }

        string T(string? key);

        string? T(string? key, bool whenNullReturnKey);

        void Initialize(string culture);

        void SetCulture(string culture);

        string ToWeek(DateTime? dateTime = null);
    }
}
