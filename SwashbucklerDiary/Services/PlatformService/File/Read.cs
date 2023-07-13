using System.Text.Json;

namespace SwashbucklerDiary.Services
{
    public partial class PlatformService
    {
        public async Task<T> ReadJsonFileAsync<T>(string path)
        {
            bool exists = await FileSystem.AppPackageFileExistsAsync(path);
            if (!exists)
            {
                return default!;
            }

            using var stream = await FileSystem.OpenAppPackageFileAsync(path).ConfigureAwait(false);
            using var reader = new StreamReader(stream);
            var contents = await reader.ReadToEndAsync().ConfigureAwait(false);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<T>(contents, options) ?? throw new("not find json file");
        }

        public async Task<string> ReadMarkdownFileAsync(string path)
        {
            bool exist = await FileSystem.AppPackageFileExistsAsync(path);
            if (!exist)
            {
                return string.Empty;
            }

            using var stream = await FileSystem.OpenAppPackageFileAsync(path);
            using var sr = new StreamReader(stream);
            var content = sr.ReadToEnd();
            return content;
        }
    }
}
