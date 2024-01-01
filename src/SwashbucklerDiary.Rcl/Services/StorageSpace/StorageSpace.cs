using SwashbucklerDiary.Rcl.Essentials;

namespace SwashbucklerDiary.Rcl.Services
{
    public abstract class StorageSpace : IStorageSpace
    {
        protected readonly IAppFileManager _appFileManager;

        public StorageSpace(IAppFileManager appFileManager) 
        {
            _appFileManager = appFileManager;
        }

        public abstract void ClearCache();

        public abstract string GetCacheSize();

        protected static string ConvertBytesToReadable(long bytes)
        {
            string[] sizes = ["B", "KB", "MB", "GB", "TB"];
            int i = 0;
            double size = bytes;

            while (size >= 1024 && i < sizes.Length - 1)
            {
                size /= 1024;
                i++;
            }

            return $"{size.ToString("0.#")} {sizes[i]}";
        }
    }
}
