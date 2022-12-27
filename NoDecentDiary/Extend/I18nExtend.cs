using BlazorComponent.I18n;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace NoDecentDiary.Extend
{
    public static class I18nExtend
    {
        public static IBlazorComponentBuilder AddI18nForMauiBlazor(this IBlazorComponentBuilder builder, string localesDirectoryApi)
        {
            string supportedCulturesApi = Path.Combine(localesDirectoryApi, "supportedCultures.json");
            bool existsCultures =  FileSystem.AppPackageFileExistsAsync(supportedCulturesApi).Result;
            if (!existsCultures)
            {
                throw new Exception("Can't find path：" + supportedCulturesApi);
            }

            using Stream streamCultures =  FileSystem.OpenAppPackageFileAsync(supportedCulturesApi).Result;
            using StreamReader readerCultures = new StreamReader(streamCultures);
            string contents = readerCultures.ReadToEnd();
            string[] cultures = JsonSerializer.Deserialize<string[]>(contents) ?? throw new Exception("Failed to read supportedCultures json file data!"); ;
            List<(string culture, Dictionary<string, string>)> locales = new List<(string, Dictionary<string, string>)>();
            string[] array = cultures;
            foreach (string culture in array)
            {
                string cultureApi = Path.Combine(localesDirectoryApi, culture + ".json");
                bool existsCulture =  FileSystem.AppPackageFileExistsAsync(cultureApi).Result;
                if (!existsCulture)
                {
                    throw new Exception("Can't find path：" + cultureApi);
                }

                using Stream stream =  FileSystem.OpenAppPackageFileAsync(cultureApi).Result;
                using StreamReader reader = new StreamReader(stream);
                Dictionary<string, string> map = I18nReader.Read(reader.ReadToEnd());
                locales.Add((culture, map));
            }

            (string culture, Dictionary<string, string>)[] localesArray = locales.ToArray();
            I18nServiceCollectionExtensions.AddI18n(builder, localesArray);
            return builder;
        }
    }
}
