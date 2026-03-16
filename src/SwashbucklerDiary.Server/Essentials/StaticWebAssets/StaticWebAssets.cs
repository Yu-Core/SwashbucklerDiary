using System.Text.Json;

namespace SwashbucklerDiary.Server.Essentials
{
    public class StaticWebAssets : Rcl.Essentials.StaticWebAssets
    {
        protected override async Task<T> ReadJsonAsyncCore<T>(string relativePath, JsonSerializerOptions options)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "wwwroot", relativePath);

            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found.", path);

            await using var stream = File.OpenRead(path);

            return await JsonSerializer.DeserializeAsync<T>(stream, options).ConfigureAwait(false)
                   ?? throw new JsonException($"Failed to deserialize json file: {path}");
        }

        protected override Task<string> ReadTextAsyncCore(string relativePath)
        {
            var path = Path.Combine(AppContext.BaseDirectory, "wwwroot", relativePath);

            if (!File.Exists(path))
                throw new FileNotFoundException($"File not found.", path);

            return File.ReadAllTextAsync(path);
        }
    }
}
