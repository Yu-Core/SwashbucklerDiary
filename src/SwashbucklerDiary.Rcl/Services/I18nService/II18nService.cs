using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace SwashbucklerDiary.Rcl.Services
{
    public interface II18nService
    {
        event EventHandler? CultureChanged;

        CultureInfo Culture { get; }

        IEnumerable<CultureInfo> SupportedCultures { get; }

        string T(string? key, params object[] args);

        string? T(string? key, [DoesNotReturnIf(true)] bool whenNullReturnKey = true, string? defaultValue = null, params object[] args);

        public void SetCulture(CultureInfo culture, CultureInfo? uiCulture = null);
    }
}
