using System.Text.Json;

namespace SwashbucklerDiary.Rcl.Essentials
{
    public interface IStaticWebAssets
    {
        Task<T> ReadRclJsonAsync<T>(string relativePath, string? assemblyName = null, JsonSerializerOptions? options = null);
        Task<T> ReadJsonAsync<T>(string relativePath, JsonSerializerOptions? options = null);
        Task<string> ReadRclTextAsync(string relativePath, string? assemblyName = null);
        Task<string> ReadTextAsync(string relativePath);
    }
}
