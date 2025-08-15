namespace SwashbucklerDiary.Rcl.Essentials
{
    public interface IAppFileSystem
    {
        public string AppDataDirectory { get; }

        public string CacheDirectory { get; }

        Task SyncFS();

        Task<string> CreateTempFileAsync(string fileName, string contents);

        Task<string> CreateTempFileAsync(string fileName, byte[] contents);

        Task<string> CreateTempFileAsync(string fileName, Stream stream);

        void FileCopy(string sourceFilePath, string targetFilePath);

        Task FileCopyAsync(Stream sourceStream, string targetFilePath);

        void FileMove(string sourceFilePath, string targetFilePath);

        Task ClearFolderAsync(string folderPath, List<string>? exceptPaths = null);

        Task<long> GetFolderSize(string folderPath);

        void MoveFolder(string sourceFolder, string destinationFolder, SearchOption searchOption, bool fileOverwrite = false);

        Task MoveFolderAsync(string sourceFolder, string destinationFolder, SearchOption searchOption, bool fileOverwrite = false);
        /// <summary>
        /// 清除缓存
        /// </summary>
        Task ClearCacheAsync();

        /// <summary>
        /// 获取缓存大小
        /// </summary>
        /// <returns></returns>
        Task<string> GetCacheSizeAsync();
    }
}
