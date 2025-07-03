using System.Globalization;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static class CultureInfoExtensions
    {
        public static string GetLanguageName(this CultureInfo cultureInfo)
        {
            return cultureInfo.Name switch
            {
                "zh-CN" => new CultureInfo("zh-Hans").NativeName,
                "zh-TW" => new CultureInfo("zh-Hant").NativeName,
                _ => cultureInfo.NativeName
            };
        }
    }
}
