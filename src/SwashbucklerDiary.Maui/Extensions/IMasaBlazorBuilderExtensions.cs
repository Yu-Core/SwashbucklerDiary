using Masa.Blazor.Core.I18n;
using System.Text.Json;

namespace SwashbucklerDiary.Maui.Extensions
{
    public static class IMasaBlazorBuilderExtensions
    {
        public static IMasaBlazorBuilder AddI18nForMauiBlazor(this IMasaBlazorBuilder builder, string localesDirectory)
        {
            string supportedCulturesPath = localesDirectory + "/supportedCultures.json";
            bool existsCultures = FileSystem.AppPackageFileExistsAsync(supportedCulturesPath).Result;
            if (!existsCultures)
            {
                throw new Exception("Can't find path：" + supportedCulturesPath);
            }

            using Stream streamCultures = FileSystem.OpenAppPackageFileAsync(supportedCulturesPath).Result;
            using StreamReader readerCultures = new(streamCultures);
            string contents = readerCultures.ReadToEnd();
            string[] cultures = JsonSerializer.Deserialize<string[]>(contents) ?? throw new Exception("Failed to read supportedCultures json file data!");
            List<(string culture, Dictionary<string, string>)> locales = [];
            foreach (string culture in cultures)
            {
                string culturePath = localesDirectory + "/" + culture + ".json";
                bool existsCulture = FileSystem.AppPackageFileExistsAsync(culturePath).Result;
                if (!existsCulture)
                {
                    throw new Exception("Can't find path：" + culturePath);
                }

                using Stream stream = FileSystem.OpenAppPackageFileAsync(culturePath).Result;
                using StreamReader reader = new(stream);
                Dictionary<string, string> map = I18nReader.Read(reader.ReadToEnd());
                locales.Add((culture, map));
            }

            I18nServiceCollectionExtensions.AddI18n(builder, locales.ToArray());
            return builder;
        }
    }
}
