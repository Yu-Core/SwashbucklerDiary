using System.Text.Json;

namespace SwashbucklerDiary.Maui.Essentials
{
    public class StaticWebAssets : Rcl.Essentials.StaticWebAssets
    {
        protected override async Task<T> ReadJsonAsyncCore<T>(string relativePath, JsonSerializerOptions options)
        {
            using var stream = await ReadStreamAsync(relativePath).ConfigureAwait(false);
            return await JsonSerializer.DeserializeAsync<T>(stream, options).ConfigureAwait(false)
                   ?? throw new JsonException($"Failed to deserialize json file: {relativePath}");
        }

        protected override async Task<string> ReadTextAsyncCore(string relativePath)
        {
            using var stream = await ReadStreamAsync(relativePath).ConfigureAwait(false);
            using var reader = new StreamReader(stream);
            return await reader.ReadToEndAsync().ConfigureAwait(false);
        }

        private static async Task<Stream> ReadStreamAsync(string relativePath)
        {
            string path = Path.Combine("wwwroot", relativePath);
            bool exists = await FileSystem.AppPackageFileExistsAsync(path).ConfigureAwait(false);
            if (!exists)
                throw new FileNotFoundException($"File not found.", path);

            return await FileSystem.OpenAppPackageFileAsync(path).ConfigureAwait(false);
        }
    }
}
