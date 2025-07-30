using Masa.Blazor;
using System.Text.RegularExpressions;

namespace SwashbucklerDiary.Rcl.Components
{
    public class PatternPathComponentBase : Masa.Blazor.Presets.PatternPathComponentBase
    {
        internal virtual PatternPath GetCurrentPatternPath()
        {
            string absolutePath = NavigationManager.GetAbsolutePath();
            Regex regex = CachedSelfPatternRegexes.FirstOrDefault((Regex r) => r.IsMatch(absolutePath));
            if (regex != null)
            {
                return new PatternPath(regex.ToString(), absolutePath);
            }

            return new PatternPath(absolutePath);
        }
    }
}
