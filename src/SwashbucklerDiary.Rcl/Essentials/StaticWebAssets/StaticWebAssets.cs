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

        public abstract Task<T> ReadJsonAsync<T>(string relativePath, bool isRcl = true, JsonSerializerOptions? jsonSerializerOptions = null);

        public abstract Task<string> ReadContentAsync(string relativePath, bool isRcl = true);
    }
}
