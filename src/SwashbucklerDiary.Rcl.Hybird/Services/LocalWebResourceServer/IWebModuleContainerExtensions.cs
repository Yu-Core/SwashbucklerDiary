using EmbedIO;
using EmbedIO.Files;

namespace SwashbucklerDiary.Rcl.Hybird.Services
{
    public static class IWebModuleContainerExtensions
    {
        public static TContainer WithStaticFolder<TContainer>(this TContainer @this, Dictionary<string, string> routeFilePathMap, Action<LocalFileModule>? configure = null) where TContainer : class, IWebModuleContainer
        {
            foreach (var item in routeFilePathMap)
            {
                @this.WithStaticFolder(item.Key, item.Value, configure);
            }

            return @this;
        }

        public static TContainer WithStaticFolder<TContainer>(this TContainer @this, string baseRoute, string fileSystemPath, Action<LocalFileModule>? configure = null) where TContainer : class, IWebModuleContainer
        {
            return @this.WithStaticFolder(null, baseRoute, fileSystemPath, configure);
        }

        public static TContainer WithStaticFolder<TContainer>(this TContainer @this, string? name, string baseRoute, string fileSystemPath, Action<LocalFileModule>? configure = null) where TContainer : class, IWebModuleContainer
        {
            FileSystemProvider fileSystemProvider = new FileSystemProvider(fileSystemPath, false);
            try
            {
                LocalFileModule module = new LocalFileModule(baseRoute, fileSystemProvider);
                return @this.WithModule(name, module, configure);
            }
            catch
            {
                fileSystemProvider.Dispose();
                throw;
            }
        }
    }
}
