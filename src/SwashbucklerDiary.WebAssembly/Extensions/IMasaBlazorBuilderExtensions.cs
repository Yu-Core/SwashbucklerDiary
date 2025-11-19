using Masa.Blazor.Core.I18n;
using System.Net.Http.Json;
using System.Text;

namespace SwashbucklerDiary.WebAssembly.Extensions
{
    public static class IMasaBlazorBuilderExtensions
    {
        public static async Task<IMasaBlazorBuilder> AddI18nForWasmAsync(this IMasaBlazorBuilder builder, string localesDirectoryApi,
        Encoding? encoding = null)
        {
            using var httpclient = new HttpClient();

            string supportedCulturesApi = Path.Combine(localesDirectoryApi, "supportedCultures.json");

            var cultures = await httpclient.GetFromJsonAsync<string[]>(supportedCulturesApi) ??
                           throw new Exception("Failed to read supportedCultures json file data!");

            var locales = new List<(string culture, Dictionary<string, string>)>();

            foreach (var culture in cultures)
            {
                await using var stream = await httpclient.GetStreamAsync(Path.Combine(localesDirectoryApi, $"{culture}.json"));
                using StreamReader reader = new StreamReader(stream);
                var jsonData = await reader.ReadToEndAsync();
                var map = I18nReader.Read(jsonData, encoding);
                locales.Add((culture, map));
            }

            I18nServiceCollectionExtensions.AddI18n(builder, locales.ToArray());

            return builder;
        }
    }
}
