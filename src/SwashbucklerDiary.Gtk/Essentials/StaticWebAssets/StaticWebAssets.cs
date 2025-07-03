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
                relativePath = Path.Combine("_content", RclAssemblyName, relativePath);
            }
#endif     
            path = Path.Combine(AppContext.BaseDirectory, "wwwroot", relativePath);

            if (!File.Exists(path))
            {
                throw new FileNotFoundException($"not find {path}");
            }

            return await File.ReadAllTextAsync(path).ConfigureAwait(false);
        }
    }
}
