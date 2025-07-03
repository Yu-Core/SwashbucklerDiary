using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static class IStaticWebAssetsExtensions
    {
        public static async Task<string> ReadI18nContentAsync(this IStaticWebAssets staticWebAssets, string relativePathFormat, object culture, bool isRcl = true)
        {
            var path = string.Format(relativePathFormat, culture);
            string content;
            try
            {
                content = await staticWebAssets.ReadContentAsync(path);
            }
            catch (FileNotFoundException)
            {
                content = await staticWebAssets.ReadContentAsync(string.Format(relativePathFormat, "en-US"), isRcl);
            }

            return content;
        }
    }
}
