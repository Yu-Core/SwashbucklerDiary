using System.Text.Json;

namespace SwashbucklerDiary.Server.Services
{
    public class StaticWebAssets : Rcl.Service.StaticWebAssets
    {
        public override async Task<T> ReadJsonAsync<T>(string relativePath)
        {
            var path = $"{AppContext.BaseDirectory}/wwwroot/_content/{RclAssemblyName}/{relativePath}";
            if (!File.Exists(path))
            {
                throw new Exception($"not find json {path}");
            }

            var contents = await File.ReadAllTextAsync(path).ConfigureAwait(false);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<T>(contents, options) ?? throw new($"{path} deserialize fail");
        }
    }
}
