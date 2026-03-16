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

        protected override async Task<T> ReadJsonAsyncCore<T>(string relativePath, JsonSerializerOptions options)
        {
            return await _httpClient.GetFromJsonAsync<T>(relativePath, options).ConfigureAwait(false)
                ?? throw new JsonException($"Failed to deserialize json from '{relativePath}'.");
        }

        protected override async Task<string> ReadTextAsyncCore(string relativePath)
        {
            var result = await _httpClient.GetStringAsync(relativePath).ConfigureAwait(false);
            return result ?? throw new FileNotFoundException($"File not found.", relativePath);
        }
    }
}
