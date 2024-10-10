using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.WebAssembly
{
    public static class FileSystem
    {
        public static readonly string AppDataDirectory = AppFileSystem.AppDataVirtualDirectoryName;

        public static readonly string CacheDirectory = AppFileSystem.CacheVirtualDirectoryName;
    }
}
