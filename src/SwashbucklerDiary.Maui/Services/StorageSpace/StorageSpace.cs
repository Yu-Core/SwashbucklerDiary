using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;

namespace SwashbucklerDiary.Maui.Services
{
    public class StorageSpace : IStorageSpace
    {
        private readonly IAppFileManager _appFileManager;

        public StorageSpace(IAppFileManager appFileManager) 
        {
            _appFileManager = appFileManager;
        }

        public void ClearCache()
            => _appFileManager.ClearFolder(FileSystem.CacheDirectory);

        public string GetCacheSize()
        {
            long fileSizeInBytes = _appFileManager.GetFolderSize(FileSystem.CacheDirectory);
            return ConvertBytesToReadable(fileSizeInBytes);
        }

        private static string ConvertBytesToReadable(long bytes)
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
