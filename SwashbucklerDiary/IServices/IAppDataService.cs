using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.IServices
{
    public interface IAppDataService
    {
        string GetBackupFileName();

        /// <summary>
        /// 清除缓存
        /// </summary>
        void ClearCache();

        /// <summary>
        /// 获取缓存大小
        /// </summary>
        /// <returns></returns>
        string GetCacheSize();

        Task<Stream> BackupDatabase(List<DiaryModel> diaries, bool copyResources);

        Task<string?> BackupDatabase(string path, List<DiaryModel> diaries, bool copyResources);

        /// <summary>
        /// 恢复数据库
        /// </summary>
        /// <param name="filePath">数据库文件路径</param>
        Task<bool> RestoreDatabase(string filePath);

        Task<bool> RestoreDatabase(Stream stream);

        Task<string> ExportTxtZipFileAsync(List<DiaryModel> diaries);

        Task<string> ExportJsonZipFileAsync(List<DiaryModel> diaries);

        Task<string> ExportMdZipFileAsync(List<DiaryModel> diaries);

        Task<string> ExportDBZipFileAsync(List<DiaryModel> diaries, bool copyResources);

        Task<string> ExportXlsxFileAsync(List<DiaryModel> diaries);

        Task<bool> ExportTxtZipFileAndSaveAsync(List<DiaryModel> diaries);

        Task<bool> ExportJsonZipFileAndSaveAsync(List<DiaryModel> diaries);

        Task<bool> ExportMdZipFileAndSaveAsync(List<DiaryModel> diaries);

        Task<bool> ExportXlsxFileAndSaveAsync(List<DiaryModel> diaries);

        Task<string> CreateCacheFileAsync(string filePath, string contents);

        Task<string> CreateCacheFileAsync(string filePath, byte[] contents);

        Task<string> CreateAppDataFileAsync(string fn, string filePath);

        Task<string> CreateAppDataFileAsync(ResourceType type, string filePath);

        Task<bool> DeleteAppDataFileByFilePathAsync(string filePath);

        Task<bool> DeleteAppDataFileByCustomPathAsync(string uri);

        void DeleteAppDataFileByCustomPath(List<string>? exceptUris, ResourceType resourceType);

        Task<List<DiaryModel>> ImportJsonFileAsync(string filePath);

        string CustomPathUriToFilePath(string uri);
    }
}
