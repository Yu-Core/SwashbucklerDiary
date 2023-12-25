using System.Net.Http.Json;

namespace SwashbucklerDiary.WebAssembly.Services
{
    public class StaticWebAssets : Rcl.Service.StaticWebAssets
    {
        private HttpClient HttpClient { get; set; }

        public StaticWebAssets(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }

        public override async Task<T> ReadJsonAsync<T>(string relativePath)
        {
            string path = $"_content/{RclAssemblyName}/{relativePath}";
            var result = await HttpClient.GetFromJsonAsync<T>(path).ConfigureAwait(false);
            return result ?? throw new Exception($"not find json {path}");
        }
    }
}
