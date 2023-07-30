using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.IServices
{
    public interface IAppDataService
    {
        string BackupFileName { get; }
        /// <summary>
        /// 清除缓存
        /// </summary>
        void ClearCache();
        /// <summary>
        /// 获取缓存大小
        /// </summary>
        /// <returns></returns>
        string GetCacheSize();
        /// <summary>
        /// 获取数据库文件
        /// </summary>
        /// <returns></returns>
        FileStream GetDatabaseStream();
        /// <summary>
        /// 恢复数据库
        /// </summary>
        /// <param name="path">数据库文件路径</param>
        void RestoreDatabase(string path);
        void RestoreDatabase(Stream stream);
        Task<string> ExportTxtFileAsync(List<DiaryModel> diaries);
        Task<string> ExportJsonFileAsync(List<DiaryModel> diaries);
        Task<string> ExportMdFileAsync(List<DiaryModel> diaries);
        Task<bool> ExportTxtFileAndSaveAsync(List<DiaryModel> diaries);
        Task<bool> ExportJsonFileAndSaveAsync(List<DiaryModel> diaries);
        Task<bool> ExportMdFileAndSaveAsync(List<DiaryModel> diaries);
        Task<string> CreateCacheFileAsync(string filePath, string contents);
        Task<string> CreateCacheFileAsync(string filePath, byte[] contents);
        Task<string> CreateAppDataFileAsync(string fn, string filePath);
        Task<string> CreateAppDataImageFileAsync(string filePath);
        Task<string> CreateAppDataAudioFileAsync(string filePath);
        Task<string> CreateAppDataVideoFileAsync(string filePath);
        Task<bool> DeleteAppDataFileByFilePathAsync(string filePath);
        Task<bool> DeleteAppDataFileByCustomSchemeAsync(string uri);
        Task<List<DiaryModel>> ImportJsonFileAsync(string filePath);
    }
}
