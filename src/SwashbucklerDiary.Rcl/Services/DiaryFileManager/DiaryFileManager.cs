using ClosedXML.Excel;
using Microsoft.Data.Sqlite;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;
using System.Data;
using System.Globalization;
using System.IO.Compression;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace SwashbucklerDiary.Rcl.Services
{
    public abstract class DiaryFileManager : IDiaryFileManager
    {
        protected readonly IAppFileSystem _appFileSystem;

        protected readonly IPlatformIntegration _platformIntegration;

        protected readonly II18nService _i18n;

        protected readonly IMediaResourceManager _mediaResourceManager;

        protected readonly IDiaryService _diaryService;

        protected readonly IResourceService _resourceService;

        protected readonly ISettingService _settingService;

        protected const string exportFileNamePrefix = "SwashbucklerDiaryExport";

        protected const string backupFileNamePrefix = "SwashbucklerDiaryBackup";

        protected const string versionInfoFileName = "version.json";

        protected const string settingsFileName = "settings.json";

        protected const string exportFileNameDateTimeFormat = "yyyyMMddHHmmss";

        protected JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        protected string DatabaseFilename => SQLiteConstants.DatabaseFilename;

        protected abstract string DatabasePath { get; }

        protected abstract string PrivacyDatabasePath { get; }

        protected static readonly List<string> excludedSettings =
        [
            nameof(Setting.WebDavConfig),
            nameof(Setting.PrivacyModeEntrancePassword),
            nameof(Setting.PrivacyModeDark),
            nameof(Setting.PrivacyModeFunctionSearchKey),
            nameof(Setting.HidePrivacyModeEntrance),
            nameof(Setting.SetPrivacyDiary),
            nameof(Setting.AppLockNumberPassword),
            nameof(Setting.AppLockPatternPassword),
            nameof(Setting.AppLockBiometric),
            nameof(Setting.LockAppWhenLeave),
            nameof(Setting.PrivacyModeEntrancePasswordSalt),
            nameof(Setting.AppLockNumberPasswordSalt),
            nameof(Setting.AppLockPatternPasswordSalt),
        ];

        public DiaryFileManager(IAppFileSystem appFileSystem,
            IPlatformIntegration platformIntegration,
            II18nService i18nService,
            IMediaResourceManager mediaResourceManager,
            IDiaryService diaryService,
            IResourceService resourceService,
            ISettingService settingService)
        {
            _appFileSystem = appFileSystem;
            _platformIntegration = platformIntegration;
            _i18n = i18nService;
            _mediaResourceManager = mediaResourceManager;
            _diaryService = diaryService;
            _resourceService = resourceService;
            _settingService = settingService;
        }

        public async Task<string> ExportDBAsync(bool copyResources)
        {
            string outputFolder = Path.Combine(_appFileSystem.CacheDirectory, "DB");
            string zipFilePath = Path.Combine(_appFileSystem.CacheDirectory, $"{backupFileNamePrefix}.zip");

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            else
            {
                await _appFileSystem.ClearFolderAsync(outputFolder).ConfigureAwait(false);
            }

            string databasePath = GetCurrentDatabasePath();
            var destFileName = Path.Combine(outputFolder, DatabaseFilename);
            SqliteConnection.ClearAllPools();
            await _appFileSystem.CopyFileAsync(databasePath,destFileName).ConfigureAwait(false);
            if (copyResources)
            {
                await CopyDiaryResourceAsync(outputFolder).ConfigureAwait(false);
                await CopyAvatarAsync(outputFolder).ConfigureAwait(false);
            }

            await CreateSettingsFileAsync(outputFolder).ConfigureAwait(false);
            await CreateExportVersionInfoAsync(outputFolder, ".db3").ConfigureAwait(false);

            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }

            await Task.Run(() => ZipFile.CreateFromDirectory(outputFolder, zipFilePath)).ConfigureAwait(false);
            return zipFilePath;
        }

        public async Task<string> ExportJsonAsync(List<DiaryModel> diaries)
        {
            string outputFolder = Path.Combine(_appFileSystem.CacheDirectory, "Json");
            string zipFilePath = Path.Combine(_appFileSystem.CacheDirectory, $"{exportFileNamePrefix}Json.zip");

            string fileSuffix = ".json";

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            else
            {
                await _appFileSystem.ClearFolderAsync(outputFolder).ConfigureAwait(false);
            }

            foreach (var item in diaries)
            {
                string fileName = item.CreateTime.ToString("yyyy-MM-dd") + fileSuffix;
                string filePath = Path.Combine(outputFolder, fileName);

                string content = JsonSerializer.Serialize(item, jsonSerializerOptions);
                await WriteToFileAsync(filePath, content).ConfigureAwait(false);
            }

            await CopyDiaryResourceAsync(diaries, outputFolder).ConfigureAwait(false);

            await CreateExportVersionInfoAsync(outputFolder, fileSuffix).ConfigureAwait(false);

            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }

            // 将所有json文件添加到压缩包中
            await Task.Run(() => ZipFile.CreateFromDirectory(outputFolder, zipFilePath)).ConfigureAwait(false);
            return zipFilePath;
        }

        public Task<string> ExportMdAsync(List<DiaryModel> diaries)
            => ExportTextFileAsync(diaries, "Markdown", ".md");

        public Task<string> ExportTxtAsync(List<DiaryModel> diaries)
            => ExportTextFileAsync(diaries, "Txt", ".txt");

        private async Task<string> ExportTextFileAsync(List<DiaryModel> diaries, string name, string fileExtension)
        {
            string outputFolder = Path.Combine(_appFileSystem.CacheDirectory, name);
            string zipFilePath = Path.Combine(_appFileSystem.CacheDirectory, $"{exportFileNamePrefix}{name}.zip");

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            else
            {
                await _appFileSystem.ClearFolderAsync(outputFolder).ConfigureAwait(false);
            }

            foreach (var item in diaries)
            {
                string fileName = item.CreateTime.ToString(exportFileNameDateTimeFormat) + fileExtension;
                string filePath = Path.Combine(outputFolder, fileName);
                await WriteToFileAsync(filePath, item.Content).ConfigureAwait(false);
            }

            await CopyDiaryResourceAsync(diaries, outputFolder).ConfigureAwait(false);

            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }

            await Task.Run(() => ZipFile.CreateFromDirectory(outputFolder, zipFilePath)).ConfigureAwait(false);
            return zipFilePath;
        }

        public Task<string> ExportXlsxAsync(List<DiaryModel> diaries)
        {
            string filePath = Path.Combine(_appFileSystem.CacheDirectory, $"{exportFileNamePrefix}Xlsx.xlsx");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var timeCol = _i18n.T("Time");
            var weatherCol = _i18n.T("Weather");
            var moodCol = _i18n.T("Mood");
            var locationCol = _i18n.T("Location");
            var tagCol = _i18n.T("Tag");
            var titleCol = _i18n.T("Title");
            var contentCol = _i18n.T("Content");

            return Task.Run(() =>
            {
                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Sheet1");

                // 添加表头
                worksheet.Cell(1, 1).Value = timeCol;
                worksheet.Cell(1, 2).Value = weatherCol;
                worksheet.Cell(1, 3).Value = moodCol;
                worksheet.Cell(1, 4).Value = locationCol;
                worksheet.Cell(1, 5).Value = tagCol;
                worksheet.Cell(1, 6).Value = titleCol;
                worksheet.Cell(1, 7).Value = contentCol;

                // 处理数据行
                for (int i = 0; i < diaries.Count; i++)
                {
                    var item = diaries[i];
                    int row = i + 2;

                    worksheet.Cell(row, 1).Value = item.CreateTime.ToString("yyyy/MM/dd HH:mm:ss");
                    worksheet.Cell(row, 2).Value = item.Weather is null ? string.Empty : _i18n.T(item.Weather);
                    worksheet.Cell(row, 3).Value = item.Mood is null ? string.Empty : _i18n.T(item.Mood);
                    worksheet.Cell(row, 4).Value = item.Location ?? string.Empty;

                    worksheet.Cell(row, 5).Value = item.Tags is not null && item.Tags.Count > 0
                        ? string.Join(", ", item.Tags.Select(it => it.Name))
                        : string.Empty;

                    worksheet.Cell(row, 6).Value = item.Title ?? string.Empty;
                    worksheet.Cell(row, 7).Value = item.Content ?? string.Empty;
                }

                workbook.SaveAs(filePath);
                return filePath;
            });
        }

        protected void CopyUriFileToOutFolder(string uri, string outFolder)
        {
            var filePath = _mediaResourceManager.RelativeUrlToFilePath(uri);
            if (!File.Exists(filePath))
            {
                return;
            }

            var outFilePath = Path.Combine(outFolder, uri.Replace('/', Path.DirectorySeparatorChar)); ;
            var outFileDir = Path.GetDirectoryName(outFilePath);
            if (outFileDir is null)
            {
                return;
            }

            if (!Directory.Exists(outFileDir))
            {
                Directory.CreateDirectory(outFileDir);
            }

            _appFileSystem.FileCopy(filePath, outFilePath);
        }

        private static async Task WriteToFileAsync(string fileName, string? content)
        {
            int suffix = 1;
            string newFileName = fileName;
            string fileDirectoryName = Path.GetDirectoryName(fileName) ?? "";
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            string pathWithoutExtension = Path.Combine(fileDirectoryName, fileNameWithoutExtension);
            string fileExtension = Path.GetExtension(fileName);

            while (File.Exists(newFileName))
            {
                suffix++;
                newFileName = $"{pathWithoutExtension}({suffix}){fileExtension}";
            }

            using StreamWriter writer = new(newFileName);
            await writer.WriteAsync(content).ConfigureAwait(false);
        }

        private async Task CreateExportVersionInfoAsync(string outputFolder, string fileSuffix)
        {
            var exportVersionInfo = new ExportVersionInfo()
            {
                Version = _platformIntegration.AppVersionString,
                FileSuffix = fileSuffix,
                Platform = _platformIntegration.CurrentPlatform.ToString(),
                DateTime = DateTime.Now,
            };

            // 将对象序列化为 JSON 字符串
            string jsonString = JsonSerializer.Serialize(exportVersionInfo);

            // 将 JSON 字符串写入文件
            var jsonPath = Path.Combine(outputFolder, versionInfoFileName);
            await File.WriteAllTextAsync(jsonPath, jsonString).ConfigureAwait(false);
        }

        private async Task CreateSettingsFileAsync(string outputFolder)
        {
            var settingsObject = _settingService.SaveSettingsToObject(it => !excludedSettings.Contains(it));
            string jsonString = JsonSerializer.Serialize(settingsObject);
            var settingsFilePath = Path.Combine(outputFolder, settingsFileName);
            await File.WriteAllTextAsync(settingsFilePath, jsonString).ConfigureAwait(false);
        }

        private async Task CopyAvatarAsync(string outputFolder)
        {
            var avatar = _settingService.Get(it => it.Avatar, null);
            if (string.IsNullOrEmpty(avatar))
            {
                return;
            }

            await Task.Run(() =>
            {
                CopyUriFileToOutFolder(avatar, outputFolder);
            }).ConfigureAwait(false);
        }

        private async Task CopyDiaryResourceAsync(List<DiaryModel> diaries, string outputFolder)
        {
            var resources = diaries.SelectMany(a => a.Resources ?? []).Distinct().ToList();
            await CopyDiaryResource(resources, outputFolder).ConfigureAwait(false);
        }

        private async Task CopyDiaryResourceAsync(string outputFolder)
        {
            var resources = await _resourceService.QueryAsync().ConfigureAwait(false);
            await CopyDiaryResource(resources, outputFolder).ConfigureAwait(false);
        }

        protected async Task CopyDiaryResource(List<ResourceModel> resources, string outputFolder)
        {
            await Task.Run(() =>
            {
                Parallel.ForEach(resources, (resource, _) =>
                {
                    if (resource.ResourceUri is null)
                    {
                        return;
                    }

                    CopyUriFileToOutFolder(resource.ResourceUri, outputFolder);
                });
            }).ConfigureAwait(false);
        }

        public string GetExportFileName(ExportKind exportKind)
        {
            string prefix = exportFileNamePrefix + exportKind.ToString();
            string suffix = exportKind == ExportKind.Xlsx ? ".xlsx" : ".zip";
            return InternalGetExportFileName(prefix, suffix);
        }

        public string GetBackupFileName()
        {
            return InternalGetExportFileName(backupFileNamePrefix, ".zip");
        }

        private string InternalGetExportFileName(string prefix, string suffix)
        {
            string dataTime = DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
            string version = _platformIntegration.AppVersionString;
            return $"{prefix}_{dataTime}_v{version}{suffix}";
        }

        public async Task<bool> ImportDBAsync(Stream stream)
        {
            string fileName = Guid.NewGuid().ToString("N") + ".zip";
            string path = await _appFileSystem.CreateTempFileAsync(fileName, stream).ConfigureAwait(false);
            var flag = await ImportDBAsync(path).ConfigureAwait(false);
            File.Delete(path);
            await _appFileSystem.SyncFS();
            return flag;
        }

        public async Task<bool> ImportDBAsync(string filePath)
        {
            string outputFolder = Path.Combine(_appFileSystem.CacheDirectory, "DB");

            if (!File.Exists(filePath))
            {
                return false;
            }

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            else
            {
                await _appFileSystem.ClearFolderAsync(outputFolder).ConfigureAwait(false);
            }

            await Task.Run(() => ZipFile.ExtractToDirectory(filePath, outputFolder)).ConfigureAwait(false);

            Version? version = GetVersion(outputFolder);

            // 获取文件夹下的db文件
            string[] dbFiles = Directory.GetFiles(outputFolder, "*.db3");
            if (dbFiles.Length == 0)
            {
                return false;
            }

            SqliteConnection.ClearAllPools();
            await _appFileSystem.CopyFileAsync(dbFiles[0], GetCurrentDatabasePath()).ConfigureAwait(false);
            if (version is null)
            {
                await AllUseNewResourceUriAsync().ConfigureAwait(false);
                await RestoreOldDiaryResourceAsync(outputFolder).ConfigureAwait(false);
            }
            else
            {
                await RestoreDiaryResourceAsync(outputFolder).ConfigureAwait(false);
            }

            string previousAvatarUri = _settingService.Get(s => s.Avatar);
            await ReadSettingsFileAsync(outputFolder).ConfigureAwait(false);
            RestoreAvatar(outputFolder, previousAvatarUri);
            await _appFileSystem.SyncFS();
            return true;
        }

        private async Task ReadSettingsFileAsync(string outputFolder)
        {
            var settingsJsonPath = Path.Combine(outputFolder, settingsFileName);
            if (!File.Exists(settingsJsonPath)) return;

            using FileStream stream = File.OpenRead(settingsJsonPath);
            var settingsObject = JsonSerializer.Deserialize<Setting>(stream);
            if (settingsObject is null) return;

            await _settingService.SetSettingsFromObjectAsync(settingsObject, it => !excludedSettings.Contains(it)).ConfigureAwait(false);
        }

        private static Version? GetVersion(string outputFolder)
        {
            var versionJsonPath = Path.Combine(outputFolder, versionInfoFileName);
            if (File.Exists(versionJsonPath))
            {
                ExportVersionInfo? exportVersionInfo = null;
                using (FileStream stream = File.OpenRead(versionJsonPath))
                {
                    exportVersionInfo = JsonSerializer.Deserialize<ExportVersionInfo>(stream);
                }

                File.Delete(versionJsonPath);
                if (exportVersionInfo?.Version is not null)
                {
                    return new Version(exportVersionInfo.Version);
                }
            }

            return null;
        }

        public async Task<bool> ImportJsonAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            string outputFolder = Path.Combine(_appFileSystem.CacheDirectory, "Json");

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            else
            {
                await _appFileSystem.ClearFolderAsync(outputFolder).ConfigureAwait(false);
            }

            ZipFile.ExtractToDirectory(filePath, outputFolder);
            Version? version = GetVersion(outputFolder);

            // 获取文件夹下的所有json文件
            string[] jsonFiles = Directory.GetFiles(outputFolder, "*.json");
            if (jsonFiles.Length == 0)
            {
                return false;
            }

            var diaries = new List<DiaryModel>();
            foreach (string jsonFile in jsonFiles)
            {
                using FileStream openStream = File.OpenRead(jsonFile);
                var diary = await JsonSerializer.DeserializeAsync<DiaryModel>(openStream).ConfigureAwait(false);
                if (diary is not null)
                {
                    diaries.Add(diary);
                }
            }

            if (version is null)
            {
                await UseNewResourceUriAsync(diaries).ConfigureAwait(false);
                await RestoreOldDiaryResourceAsync(outputFolder).ConfigureAwait(false);
            }
            else
            {
                await RestoreDiaryResourceAsync(outputFolder).ConfigureAwait(false);
            }

            return await _diaryService.ImportAsync(diaries).ConfigureAwait(false);
        }

        public async Task<bool> ImportMdAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return false;
            }

            string outputFolder = Path.Combine(_appFileSystem.CacheDirectory, "Markdown");

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            else
            {
                await _appFileSystem.ClearFolderAsync(outputFolder).ConfigureAwait(false);
            }

            await Task.Run(() => ZipFile.ExtractToDirectory(filePath, outputFolder)).ConfigureAwait(false);

            string[] mdFilePaths = Directory.GetFiles(outputFolder, "*.md", SearchOption.AllDirectories);
            if (mdFilePaths.Length == 0)
            {
                return false;
            }

            var diaries = new List<DiaryModel>();
            foreach (string mdFilePath in mdFilePaths)
            {
                var diary = await ConvertToDiaryAsync(mdFilePath).ConfigureAwait(false);

                if (diary is not null)
                {
                    diaries.Add(diary);
                }
            }

            await RestoreDiaryResourceAsync(outputFolder).ConfigureAwait(false);

            return await _diaryService.ImportAsync(diaries).ConfigureAwait(false);
        }

        private async Task<DiaryModel> ConvertToDiaryAsync(string filePath)
        {
            var diary = new DiaryModel();
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            string dateTimeString = fileNameWithoutExtension.Split('(')[0];
            diary.CreateTime = diary.UpdateTime = ConvertToDateTime(dateTimeString);
            diary.Content = await File.ReadAllTextAsync(filePath).ConfigureAwait(false);
            diary.Resources = _mediaResourceManager.GetDiaryResources(diary.Content);
            return diary;
        }

        private static DateTime ConvertToDateTime(string dateTimeString)
        {
            if (DateTime.TryParseExact(dateTimeString, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
            {
                return dateTime;
            }

            if (DateTime.TryParse(dateTimeString, CultureInfo.InvariantCulture, out DateTime dateTime2))
            {
                return dateTime2;
            }

            return DateTime.Now;
        }

        protected Task RestoreDiaryResourceAsync(string outputFolder)
            => RestoreFoldersAsync(outputFolder, _mediaResourceManager.MediaResourceFolders.Values);

        protected async Task RestoreFoldersAsync(string outputFolder, IEnumerable<string> folderNames)
        {
            foreach (var item in folderNames)
            {
                var sourceDir = Path.Combine(outputFolder, AppFileSystem.AppDataVirtualDirectoryName, item);
                if (!Directory.Exists(sourceDir))
                {
                    continue;
                }

                var targetDir = Path.Combine(_appFileSystem.AppDataDirectory, item);
                await _appFileSystem.MoveFolderAsync(sourceDir, targetDir, SearchOption.AllDirectories).ConfigureAwait(false);
            }
        }

        protected async Task RestoreOldDiaryResourceAsync(string outputFolder)
        {
            var sourceDir = Path.Combine(outputFolder, "Image");
            if (!Directory.Exists(sourceDir))
            {
                return;
            }

            var targetDir = Path.Combine(_appFileSystem.AppDataDirectory, "Image");
            await _appFileSystem.MoveFolderAsync(sourceDir, targetDir, SearchOption.TopDirectoryOnly).ConfigureAwait(false);
        }

        private void RestoreAvatar(string outputFolder, string previousAvatarUri)
        {
            string avatarUri = _settingService.Get(s => s.Avatar);
            string avatarFileName = Path.GetFileName(avatarUri);
            string sourceFilePath = Path.Combine(outputFolder, AppFileSystem.AppDataVirtualDirectoryName, AvatarService.AvatarDirectoryName, avatarFileName);
            if (!File.Exists(sourceFilePath))
            {
                return;
            }

            string targetFilePath = Path.Combine(_appFileSystem.AppDataDirectory, AvatarService.AvatarDirectoryName, avatarFileName);
            if (File.Exists(targetFilePath))
            {
                File.Delete(targetFilePath);
            }

            _appFileSystem.FileMove(sourceFilePath, targetFilePath);
            if (avatarUri != previousAvatarUri)
            {
                string previousAvatarPath = _mediaResourceManager.RelativeUrlToFilePath(previousAvatarUri);
                if (File.Exists(previousAvatarPath))
                {
                    File.Delete(previousAvatarPath);
                }
            }
        }

        public async Task UseNewResourceUriAsync(List<DiaryModel> diaries)
        {
            await Task.Run(() =>
            {
                foreach (var diary in diaries)
                {
                    if (!string.IsNullOrEmpty(diary.Content))
                    {
                        diary.Content = diary.Content.Replace("appdata:///", "appdata/");
                        diary.Resources = _mediaResourceManager.GetDiaryResources(diary.Content);
                    }
                }
            }).ConfigureAwait(false);
        }

        public async Task AllUseNewResourceUriAsync()
        {
            var diaries = await _diaryService.QueryDiariesAsync().ConfigureAwait(false);
            await _resourceService.DeleteAsync(it => it.ResourceUri.StartsWith("appdata:///")).ConfigureAwait(false);
            await UseNewResourceUriAsync(diaries).ConfigureAwait(false);
            await _diaryService.UpdateIncludesAsync(diaries).ConfigureAwait(false);
        }

        private string GetCurrentDatabasePath()
        {
            bool privacyMode = _settingService.GetTemp(it => it.PrivacyMode);
            return privacyMode ? PrivacyDatabasePath : DatabasePath;
        }

        public async Task<string> ExportResourceFileAsync(List<ResourceModel> resources)
        {
            string outputFolder = Path.Combine(_appFileSystem.CacheDirectory, "ResourceFile");
            string zipFilePath = Path.Combine(_appFileSystem.CacheDirectory, $"{exportFileNamePrefix}ResourceFile.zip");

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            else
            {
                await _appFileSystem.ClearFolderAsync(outputFolder).ConfigureAwait(false);
            }

            await CopyDiaryResource(resources, outputFolder).ConfigureAwait(false);

            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }

            await Task.Run(() => ZipFile.CreateFromDirectory(outputFolder, zipFilePath)).ConfigureAwait(false);
            return zipFilePath;
        }

        public async Task UpdateAllDiariesResourcesAsync()
        {
            var diaries = await _diaryService.QueryDiariesAsync().ConfigureAwait(false);
            await Task.Run(() =>
            {
                foreach (var diary in diaries)
                {
                    if (!string.IsNullOrEmpty(diary.Content))
                    {
                        diary.Resources = _mediaResourceManager.GetDiaryResources(diary.Content);
                    }
                }
            }).ConfigureAwait(false);
            await _diaryService.UpdateIncludesAsync(diaries).ConfigureAwait(false);
        }
    }
}
