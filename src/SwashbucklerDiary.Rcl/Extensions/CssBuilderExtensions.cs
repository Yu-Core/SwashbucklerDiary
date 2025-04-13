using Masa.Blazor.Core;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static class CssBuilderExtensions
    {
        public static CssBuilder AddIf(this CssBuilder cssBuilder, string? value, bool condition, bool important = false)
        {
            return condition ? cssBuilder.Add(value, important) : cssBuilder;
        }

        public static string Build(this CssBuilder cssBuilder)
        {
            return cssBuilder.ToString();
        }
    }
}
