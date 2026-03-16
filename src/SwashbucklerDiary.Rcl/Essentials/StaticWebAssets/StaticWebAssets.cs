using System.Text.Json;
using System.Text.Json.Serialization;

namespace SwashbucklerDiary.Rcl.Essentials
{
    public abstract class StaticWebAssets : IStaticWebAssets
    {
        private static readonly Lazy<string> _rclAssemblyName = new(() => typeof(StaticWebAssets).Assembly.GetName().Name!);

        public static string RclAssemblyName => _rclAssemblyName.Value;

        protected virtual JsonSerializerOptions DefaultJsonSerializerOptions { get; set; } = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters =
            {
                new JsonStringEnumConverter()
            }
        };

        public Task<T> ReadRclJsonAsync<T>(string relativePath, string? assemblyName = null, JsonSerializerOptions? options = null)
            => ReadJsonAsync<T>($"_content/{assemblyName ?? RclAssemblyName}/{relativePath}", options);

        public Task<T> ReadJsonAsync<T>(string relativePath, JsonSerializerOptions? options = null)
            => ReadJsonAsyncCore<T>(NormalizePath(relativePath), options ?? DefaultJsonSerializerOptions);

        public Task<string> ReadRclTextAsync(string relativePath, string? assemblyName = null)
            => ReadTextAsync($"_content/{assemblyName ?? RclAssemblyName}/{relativePath}");

        public Task<string> ReadTextAsync(string relativePath)
            => ReadTextAsyncCore(NormalizePath(relativePath));

        protected abstract Task<T> ReadJsonAsyncCore<T>(string relativePath, JsonSerializerOptions options);
        protected abstract Task<string> ReadTextAsyncCore(string relativePath);

        private static string NormalizePath(string filename) =>
            filename
                .Replace('\\', Path.DirectorySeparatorChar)
                .Replace('/', Path.DirectorySeparatorChar);
    }
}
