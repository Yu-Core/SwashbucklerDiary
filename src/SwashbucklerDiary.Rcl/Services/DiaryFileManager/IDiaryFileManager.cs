using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public interface IDiaryFileManager
    {
        Task<string> GetExportFileName(ExportKind exportKind);

        Task<string> GetBackupFileName();

        Task<string> ExportDBAsync(bool copyResources);

        Task<string> ExportJsonAsync(List<DiaryModel> diaries);

        Task<string> ExportMdAsync(List<DiaryModel> diaries);

        Task<string> ExportTxtAsync(List<DiaryModel> diaries);

        Task<string> ExportXlsxAsync(List<DiaryModel> diaries);

        Task<bool> ImportDBAsync(string filePath);

        Task<bool> ImportDBAsync(Stream stream);

        Task<bool> ImportJsonAsync(string filePath);
    }
}
