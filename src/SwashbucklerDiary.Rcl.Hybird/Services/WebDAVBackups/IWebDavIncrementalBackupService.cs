namespace SwashbucklerDiary.Rcl.Services
{
    public interface IWebDavIncrementalBackupService
    {
        Task<WebDavIncrementalBackupManifest> UploadAsync(bool includeDiaryResources);

        Task<List<WebDavIncrementalBackupManifest>> GetManifestsAsync();

        Task<WebDavBackupDiagnostics> DiagnoseAsync();

        Task RestoreAsync(WebDavIncrementalBackupManifest manifest);
    }
}
