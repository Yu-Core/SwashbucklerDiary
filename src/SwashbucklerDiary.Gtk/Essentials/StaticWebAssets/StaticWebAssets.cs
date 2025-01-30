using System.Text.Json;

namespace SwashbucklerDiary.Gtk.Essentials
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
#if !DEBUG
            if (isRcl)
            {
                path = $"wwwroot/_content/{RclAssemblyName}/{relativePath}";
            }
            else
            {
#endif
            path = $"wwwroot/{relativePath}";
#if !DEBUG
            }
#endif
            if (!File.Exists(path))
            {
                throw new Exception($"not find {path}");
            }

            return await File.ReadAllTextAsync(path).ConfigureAwait(false);
        }
    }
}
