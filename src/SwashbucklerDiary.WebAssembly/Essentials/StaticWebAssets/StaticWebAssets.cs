using System.Net.Http.Json;
using System.Text.Json;

namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class StaticWebAssets : Rcl.Essentials.StaticWebAssets
    {
        private readonly HttpClient _httpClient;

        public StaticWebAssets(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private readonly JsonSerializerOptions defaultJsonSerializerOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public override async Task<T> ReadJsonAsync<T>(string relativePath, bool isRcl = true, JsonSerializerOptions? jsonSerializerOptions = null)
        {
            string path = RelativePathToPath(relativePath, isRcl);
            var result = await _httpClient.GetFromJsonAsync<T>(path, jsonSerializerOptions ?? defaultJsonSerializerOptions);
            return result ?? throw new($"{relativePath} deserialize fail");
        }

        public override async Task<string> ReadContentAsync(string relativePath, bool isRcl = true)
        {
            string path = RelativePathToPath(relativePath, isRcl);
            var result = await _httpClient.GetStringAsync(path);
            return result ?? throw new Exception($"not find json {path}");
        }

        private static string RelativePathToPath(string relativePath, bool isRcl)
            => isRcl ? $"_content/{RclAssemblyName}/{relativePath}" : relativePath;
    }
}
