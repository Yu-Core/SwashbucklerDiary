using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Rcl.Services;
using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.WebAssembly.Services
{
    public class DiaryFileManager : Rcl.Services.DiaryFileManager
    {
        public DiaryFileManager(IAppFileManager appFileManager,
            IPlatformIntegration platformIntegration,
            II18nService i18nService,
            IMediaResourceManager mediaResourceManager,
            IDiaryService diaryService,
            IResourceService resourceService)
            : base(appFileManager, platformIntegration, i18nService, mediaResourceManager, diaryService, resourceService)
        {
        }

        public override Task<string> ExportDBAsync(bool copyResources)
        {
            string outputFolder = Path.Combine(FileSystem.CacheDirectory, "DB");
            string zipFilePath = Path.Combine(FileSystem.CacheDirectory, $"{backupFileNamePrefix}.zip");
            return InternalExportDBAsync(copyResources, outputFolder, zipFilePath);
        }

        public override Task<string> ExportJsonAsync(List<DiaryModel> diaries)
        {
            string outputFolder = Path.Combine(FileSystem.CacheDirectory, "Json");
            string zipFilePath = Path.Combine(FileSystem.CacheDirectory, $"{exportFileNamePrefix}Json.zip");
            return InternalExportJsonAsync(diaries, outputFolder, zipFilePath);
        }

        public override Task<string> ExportMdAsync(List<DiaryModel> diaries)
        {
            string outputFolder = Path.Combine(FileSystem.CacheDirectory, "Markdown");
            string zipFilePath = Path.Combine(FileSystem.CacheDirectory, $"{exportFileNamePrefix}Markdown.zip");
            return InternalExportMdAsync(diaries, outputFolder, zipFilePath);
        }

        public override Task<string> ExportTxtAsync(List<DiaryModel> diaries)
        {
            string outputFolder = Path.Combine(FileSystem.CacheDirectory, "Txt");
            string zipFilePath = Path.Combine(FileSystem.CacheDirectory, $"{exportFileNamePrefix}Txt.zip");
            return InternalExportTxtAsync(diaries, outputFolder, zipFilePath);
        }

        public override Task<string> ExportXlsxAsync(List<DiaryModel> diaries)
        {
            string filePath = Path.Combine(FileSystem.CacheDirectory, $"{exportFileNamePrefix}Xlsx.xlsx");
            return InternalExportXlsxAsync(diaries, filePath);
        }

        protected override void CopyUriFileToOutFolder(string uri, string outFolder)
        {
            var filePath = uri;
            if (!File.Exists(filePath))
            {
                return;
            }

            var outFilePath = Path.Combine(outFolder, uri);
            var outFileDir = Path.GetDirectoryName(outFilePath);
            if (outFileDir is null)
            {
                return;
            }

            if (!Directory.Exists(outFileDir))
            {
                Directory.CreateDirectory(outFileDir);
            }

            File.Copy(filePath, outFilePath, true);
        }

        protected override void CopyDiaryResource(string outputFolder)
        {
            foreach (var item in _mediaResourceManager.MediaResourceFolders.Values)
            {
                var sourceDir = Path.Combine(FileSystem.AppDataDirectory, item);
                if (!Directory.Exists(sourceDir))
                {
                    continue;
                }

                var targetDir = Path.Combine(outputFolder, "appdata", item);
                _appFileManager.CopyFolder(sourceDir, targetDir, SearchOption.TopDirectoryOnly);
            }
        }

        public override Task<bool> ImportDBAsync(string filePath)
        {
            string outputFolder = Path.Combine(FileSystem.CacheDirectory, "DB");
            return InternalImportDBAsync(filePath, outputFolder);
        }

        protected override void ClearAllDiaryResources()
        {
            foreach (var item in _mediaResourceManager.MediaResourceFolders.Values)
            {
                var folderPath = Path.Combine(FileSystem.AppDataDirectory, item);
                _appFileManager.ClearFolder(folderPath);
            }
        }

        protected override void RestoreDiaryResource(string outputFolder)
        {
            foreach (var item in _mediaResourceManager.MediaResourceFolders.Values)
            {
                var sourceDir = Path.Combine(outputFolder, "appdata", item);
                if (!Directory.Exists(sourceDir))
                {
                    continue;
                }

                var targetDir = Path.Combine(FileSystem.AppDataDirectory, item);
                _appFileManager.MoveFolder(sourceDir, targetDir, SearchOption.TopDirectoryOnly);
            }
        }

        protected override void RestoreOldDiaryResource(string outputFolder)
        {
            var sourceDir = Path.Combine(outputFolder, "Image");
            if (!Directory.Exists(sourceDir))
            {
                return;
            }

            var targetDir = Path.Combine(FileSystem.AppDataDirectory, "Image");
            _appFileManager.MoveFolder(sourceDir, targetDir, SearchOption.TopDirectoryOnly);
        }

        protected override string GetDatabasePath()
        {
            return SQLiteConstants.DatabasePath;
        }

        public override Task<bool> ImportJsonAsync(string filePath)
        {
            string outputFolder = Path.Combine(FileSystem.CacheDirectory, "Json");
            return InternalImportJsonAsync(filePath, outputFolder);
        }
    }
}
