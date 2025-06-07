using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public interface IDiaryFileManager
    {
        string GetExportFileName(ExportKind exportKind);

        string GetBackupFileName();

        Task<string> ExportDBAsync(bool copyResources);

        Task<string> ExportJsonAsync(List<DiaryModel> diaries);

        Task<string> ExportMdAsync(List<DiaryModel> diaries);

        Task<string> ExportTxtAsync(List<DiaryModel> diaries);

        Task<string> ExportXlsxAsync(List<DiaryModel> diaries);

        string ExportResourceFile(List<ResourceModel> resources);

        Task<bool> ImportDBAsync(string filePath);

        Task<bool> ImportDBAsync(Stream stream);

        Task<bool> ImportJsonAsync(string filePath);

        Task<bool> ImportMdAsync(string filePath);

        void UseNewResourceUri(List<DiaryModel> diaries);

        Task AllUseNewResourceUriAsync();

        Task UpdateTemplateForOldDiaryAsync();
    }
}
