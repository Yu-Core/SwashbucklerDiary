namespace SwashbucklerDiary.Rcl.Essentials
{
    public abstract class StaticWebAssets : IStaticWebAssets
    {
        private static readonly Lazy<string?> _rclAssemblyName = new(() => typeof(StaticWebAssets).Assembly.GetName().Name);

        public static string? RclAssemblyName => _rclAssemblyName.Value;

        public abstract Task<T> ReadJsonAsync<T>(string relativePath, bool isRcl = true);

        public abstract Task<string> ReadContentAsync(string relativePath, bool isRcl = true);
    }
}
