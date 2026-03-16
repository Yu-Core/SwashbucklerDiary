using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Extensions
{
    public static class IStaticWebAssetsExtensions
    {
        public static Task<string> ReadRclI18nTextAsync(this IStaticWebAssets staticWebAssets, string relativePathFormat, object culture, string? assemblyName = null)
            => staticWebAssets.ReadI18nTextAsync($"_content/{assemblyName ?? StaticWebAssets.RclAssemblyName}/{relativePathFormat}", culture);

        public static async Task<string> ReadI18nTextAsync(this IStaticWebAssets staticWebAssets, string relativePathFormat, object culture)
        {
            var path = string.Format(relativePathFormat, culture);
            string content;
            try
            {
                content = await staticWebAssets.ReadTextAsync(path);
            }
            catch (FileNotFoundException)
            {
                content = await staticWebAssets.ReadTextAsync(string.Format(relativePathFormat, "en-US"));
            }

            return content;
        }
    }
}
