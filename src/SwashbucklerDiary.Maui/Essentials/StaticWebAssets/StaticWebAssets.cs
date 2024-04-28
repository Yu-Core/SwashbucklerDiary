using System.Text.Json;

namespace SwashbucklerDiary.Maui.Essentials
{
    public class StaticWebAssets : Rcl.Essentials.StaticWebAssets
    {
        public override async Task<T> ReadJsonAsync<T>(string relativePath, bool isRcl = true, JsonSerializerOptions? jsonSerializerOptions = null)
        {
            var contents = await ReadContentAsync(relativePath, isRcl).ConfigureAwait(false);
            return JsonSerializer.Deserialize<T>(contents, jsonSerializerOptions ?? DefaultJsonSerializerOptions) ?? throw new($"{relativePath} deserialize fail");
        }

        public override async Task<string> ReadContentAsync(string relativePath, bool isRcl = true)
        {
            string path;
            if (isRcl)
            {
                path = $"wwwroot/_content/{RclAssemblyName}/{relativePath}";
            }
            else
            {
                path = $"wwwroot/{relativePath}";
            }

            bool exists = await FileSystem.AppPackageFileExistsAsync(path).ConfigureAwait(false);
            if (!exists)
            {
                throw new Exception($"not find {path}");
            }

            using var stream = await FileSystem.OpenAppPackageFileAsync(path).ConfigureAwait(false);
            using var reader = new StreamReader(stream);
            return reader.ReadToEnd();
        }
    }
}
