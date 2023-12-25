namespace SwashbucklerDiary.Rcl.Essentials
{
    public interface IAppFileManager
    {
        Task<string> CreateTempFileAsync(string fileName, string contents);

        Task<string> CreateTempFileAsync(string fileName, byte[] contents);

        Task<string> CreateTempFileAsync(string fileName, Stream stream);

        Task FileCopyAsync(string targetFilePath, string sourceFilePath);

        Task FileCopyAsync(string targetFilePath, Stream sourceStream);

        Task FileMoveAsync(string sourceFilePath, string targetFilePath);

        void ClearFolder(string folderPath, List<string>? exceptPaths = null);

        long GetFolderSize(string folderPath);

        void CopyFolder(string sourceFolder, string destinationFolder, SearchOption searchOption);
    }
}
