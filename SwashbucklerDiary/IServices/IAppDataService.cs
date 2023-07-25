using SwashbucklerDiary.Models;

namespace SwashbucklerDiary.IServices
{
    public interface IAppDataService
    {
        string BackupFileName { get; }
        void ClearCache();
        string GetCacheSize();
        FileStream GetDatabaseStream();
        void RestoreDatabase(string path);
        void RestoreDatabase(Stream stream);
        Task<string> CreateTxtFileAsync(List<DiaryModel> diaries);
        Task<string> CreateJsonFileAsync(List<DiaryModel> diaries);
        Task<string> CreateMdFileAsync(List<DiaryModel> diaries);
        Task<bool> CreateTxtFileAndSaveAsync(List<DiaryModel> diaries);
        Task<bool> CreateJsonFileAndSaveAsync(List<DiaryModel> diaries);
        Task<bool> CreateMdFileAndSaveAsync(List<DiaryModel> diaries);
        Task<string> CreateCacheFileAsync(string filePath, string contents);
        Task<string> CreateCacheFileAsync(string filePath, byte[] contents);
        Task<string> CreateAppDataFileAsync(string relativePath, string sourcePath);
        Task<string> CreateAppDataImageFileAsync(string filePath);
        Task<bool> DeleteAppDataFileByFilePathAsync(string filePath);
        Task<bool> DeleteAppDataFileByCustomSchemeAsync(string uri);
    }
}
