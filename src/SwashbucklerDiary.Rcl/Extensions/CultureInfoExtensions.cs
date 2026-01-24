using System.Globalization;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static class CultureInfoExtensions
    {
        private static readonly string[] vditorSupportLanguages = ["de_DE", "en_US", "es_ES", "fr_FR", "ja_JP", "ko_KR", "pt_BR", "ru_RU", "sv_SE", "vi_VN", "zh_CN", "zh_TW"];

        public static string GetLanguageName(this CultureInfo cultureInfo)
        {
            return cultureInfo.Name switch
            {
                "zh-CN" => new CultureInfo("zh-Hans").NativeName,
                "zh-TW" => new CultureInfo("zh-Hant").NativeName,
                _ => cultureInfo.NativeName
            };
        }

        public static string ToVditorLang(this CultureInfo cultureInfo)
        {
            string lang = cultureInfo.Name.Replace("-", "_");
            return vditorSupportLanguages.Contains(lang) ? lang : "en_US";
        }
    }
}
