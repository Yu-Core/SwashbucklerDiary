using ClosedXML.Excel;
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

        protected abstract string DatabaseFilename { get; }

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
                _appFileSystem.ClearFolder(outputFolder);
            }

            string databasePath = GetCurrentDatabasePath();
            var destFileName = Path.Combine(outputFolder, DatabaseFilename);
            File.Copy(databasePath, destFileName);
            if (copyResources)
            {
                await CopyDiaryResource(outputFolder);
                CopyAvatar(outputFolder);
            }

            CreateSettingsFile(outputFolder);
            CreateExportVersionInfo(outputFolder, ".db3");

            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }

            ZipFile.CreateFromDirectory(outputFolder, zipFilePath);
            return zipFilePath;
        }

        public Task<string> ExportJsonAsync(List<DiaryModel> diaries)
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
                _appFileSystem.ClearFolder(outputFolder);
            }

            foreach (var item in diaries)
            {
                string fileName = item.CreateTime.ToString("yyyy-MM-dd") + fileSuffix;
                string filePath = Path.Combine(outputFolder, fileName);

                string content = JsonSerializer.Serialize(item, jsonSerializerOptions);
                WriteToFile(filePath, content);
            }

            CopyDiaryResource(diaries, outputFolder);

            CreateExportVersionInfo(outputFolder, fileSuffix);

            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }

            // 将所有json文件添加到压缩包中
            ZipFile.CreateFromDirectory(outputFolder, zipFilePath);
            return Task.FromResult(zipFilePath);
        }

        public Task<string> ExportMdAsync(List<DiaryModel> diaries)
            => ExportTextFileAsync(diaries, "Markdown", ".md");

        public Task<string> ExportTxtAsync(List<DiaryModel> diaries)
            => ExportTextFileAsync(diaries, "Txt", ".txt");

        private Task<string> ExportTextFileAsync(List<DiaryModel> diaries, string name, string fileExtension)
        {
            string outputFolder = Path.Combine(_appFileSystem.CacheDirectory, name);
            string zipFilePath = Path.Combine(_appFileSystem.CacheDirectory, $"{exportFileNamePrefix}{name}.zip");

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            else
            {
                _appFileSystem.ClearFolder(outputFolder);
            }

            foreach (var item in diaries)
            {
                string fileName = item.CreateTime.ToString(exportFileNameDateTimeFormat) + fileExtension;
                string filePath = Path.Combine(outputFolder, fileName);
                WriteToFile(filePath, item.Content);
            }

            CopyDiaryResource(diaries, outputFolder);

            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }

            ZipFile.CreateFromDirectory(outputFolder, zipFilePath);
            return Task.FromResult(zipFilePath);
        }

        public Task<string> ExportXlsxAsync(List<DiaryModel> diaries)
        {
            string filePath = Path.Combine(_appFileSystem.CacheDirectory, $"{exportFileNamePrefix}Xlsx.xlsx");

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var dataTable = new DataTable();
            dataTable.Columns.Add(_i18n.T("Time"));
            dataTable.Columns.Add(_i18n.T("Weather"));
            dataTable.Columns.Add(_i18n.T("Mood"));
            dataTable.Columns.Add(_i18n.T("Location"));
            dataTable.Columns.Add(_i18n.T("Tag"));
            dataTable.Columns.Add(_i18n.T("Title"));
            dataTable.Columns.Add(_i18n.T("Content"));

            foreach (var item in diaries)
            {
                var time = item.CreateTime.ToString("yyyy/MM/dd HH:mm:ss");
                var weather = item.Weather is null ? string.Empty : _i18n.T(item.Weather);
                var mood = item.Mood is null ? string.Empty : _i18n.T(item.Mood);
                var location = item.Location ?? string.Empty;
                var tags = string.Empty;
                if (item.Tags is not null && item.Tags.Count > 0)
                {
                    tags = string.Join(", ", item.Tags.Select(it => it.Name));
                }

                var title = item.Title ?? string.Empty;
                var content = item.Content ?? string.Empty;

                dataTable.Rows.Add(time, weather, mood, location, tags, title, content);
            }

            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add(dataTable, "Sheet1");
            workbook.SaveAs(filePath);
            return Task.FromResult(filePath);
        }

        protected void CopyUriFileToOutFolder(string uri, string outFolder)
        {
            var filePath = _mediaResourceManager.UrlRelativePathToFilePath(uri);
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

            File.Copy(filePath, outFilePath, true);
        }

        private static void WriteToFile(string fileName, string? content)
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
            writer.Write(content);
        }

        private void CreateExportVersionInfo(string outputFolder, string fileSuffix)
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
            File.WriteAllText(jsonPath, jsonString);
        }

        private void CreateSettingsFile(string outputFolder)
        {
            var settingsObject = _settingService.SaveSettingsToObject(it => !excludedSettings.Contains(it));
            string jsonString = JsonSerializer.Serialize(settingsObject);
            var settingsFilePath = Path.Combine(outputFolder, settingsFileName);
            File.WriteAllText(settingsFilePath, jsonString);
        }

        private void CopyAvatar(string outputFolder)
        {
            var avatar = _settingService.Get(it => it.Avatar, null);
            if (string.IsNullOrEmpty(avatar))
            {
                return;
            }

            CopyUriFileToOutFolder(avatar, outputFolder);
        }

        private void CopyDiaryResource(List<DiaryModel> diaries, string outputFolder)
        {
            var resources = diaries.SelectMany(a => a.Resources ?? []).Distinct().ToList();

            foreach (var resource in resources)
            {
                if (resource.ResourceUri is null)
                {
                    continue;
                }

                CopyUriFileToOutFolder(resource.ResourceUri, outputFolder);
            }
        }

        protected async Task CopyDiaryResource(string outputFolder)
        {
            var resources = await _resourceService.QueryAsync();
            foreach (var resource in resources)
            {
                if (resource.ResourceUri is null)
                {
                    continue;
                }

                CopyUriFileToOutFolder(resource.ResourceUri, outputFolder);
            }
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
            string dataTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            string version = _platformIntegration.AppVersionString;
            return $"{prefix}_{dataTime}_v{version}{suffix}";
        }

        public async Task<bool> ImportDBAsync(Stream stream)
        {
            string fileName = Guid.NewGuid().ToString() + ".zip";
            string path = await _appFileSystem.CreateTempFileAsync(fileName, stream);
            var flag = await ImportDBAsync(path);
            File.Delete(path);
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
                _appFileSystem.ClearFolder(outputFolder);
            }

            ZipFile.ExtractToDirectory(filePath, outputFolder);

            Version? version = GetVersion(outputFolder);

            // 获取文件夹下的db文件
            string[] dbFiles = Directory.GetFiles(outputFolder, "*.db3");
            if (dbFiles.Length == 0)
            {
                return false;
            }

            File.Copy(dbFiles[0], GetCurrentDatabasePath(), true);
            if (version is null)
            {
                await AllUseNewResourceUriAsync();
                RestoreOldDiaryResource(outputFolder);
            }
            else
            {
                RestoreDiaryResource(outputFolder);
                if (version <= new Version("1.16.8"))
                {
                    await UpdateTemplateForOldDiaryAsync();
                }
            }

            string previousAvatarUri = _settingService.Get(s => s.Avatar);
            await ReadSettingsFile(outputFolder);
            await RestoreAvatar(outputFolder, previousAvatarUri);
            return true;
        }

        private async Task ReadSettingsFile(string outputFolder)
        {
            var settingsJsonPath = Path.Combine(outputFolder, settingsFileName);
            if (!File.Exists(settingsJsonPath)) return;

            using FileStream stream = File.OpenRead(settingsJsonPath);
            var settingsObject = JsonSerializer.Deserialize<Setting>(stream);
            if (settingsObject is null) return;

            await _settingService.SetSettingsFromObjectAsync(settingsObject, it => !excludedSettings.Contains(it));
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
                _appFileSystem.ClearFolder(outputFolder);
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
                var diary = await JsonSerializer.DeserializeAsync<DiaryModel>(openStream);
                if (diary is not null)
                {
                    diaries.Add(diary);
                }
            }

            if (version is null)
            {
                UseNewResourceUri(diaries);
                RestoreOldDiaryResource(outputFolder);
            }
            else
            {
                RestoreDiaryResource(outputFolder);
            }

            return await _diaryService.ImportAsync(diaries);
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
                _appFileSystem.ClearFolder(outputFolder);
            }

            ZipFile.ExtractToDirectory(filePath, outputFolder);

            string[] mdFilePaths = Directory.GetFiles(outputFolder, "*.md", SearchOption.AllDirectories);
            if (mdFilePaths.Length == 0)
            {
                return false;
            }

            var diaries = new List<DiaryModel>();
            foreach (string mdFilePath in mdFilePaths)
            {
                var diary = ConvertToDiary(mdFilePath);

                if (diary is not null)
                {
                    diaries.Add(diary);
                }
            }

            RestoreDiaryResource(outputFolder);

            return await _diaryService.ImportAsync(diaries);
        }

        private DiaryModel ConvertToDiary(string filePath)
        {
            var diary = new DiaryModel();
            var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
            string dateTimeString = fileNameWithoutExtension.Split('(')[0];
            diary.CreateTime = diary.UpdateTime = ConvertToDateTime(dateTimeString);
            diary.Content = File.ReadAllText(filePath);
            diary.Resources = _mediaResourceManager.GetDiaryResources(diary.Content);
            return diary;
        }

        private static DateTime ConvertToDateTime(string dateTimeString)
        {
            if (DateTime.TryParseExact(dateTimeString, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime dateTime))
            {
                return dateTime;
            }

            if (DateTime.TryParse(dateTimeString, out DateTime dateTime2))
            {
                return dateTime2;
            }

            return DateTime.Now;
        }

        protected void RestoreDiaryResource(string outputFolder)
            => RestoreFolders(outputFolder, _mediaResourceManager.MediaResourceFolders.Values);

        protected void RestoreFolders(string outputFolder, IEnumerable<string> folderNames)
        {
            foreach (var item in folderNames)
            {
                var sourceDir = Path.Combine(outputFolder, AppFileSystem.AppDataVirtualDirectoryName, item);
                if (!Directory.Exists(sourceDir))
                {
                    continue;
                }

                var targetDir = Path.Combine(_appFileSystem.AppDataDirectory, item);
                _appFileSystem.MoveFolder(sourceDir, targetDir, SearchOption.TopDirectoryOnly);
            }
        }

        protected void RestoreOldDiaryResource(string outputFolder)
        {
            var sourceDir = Path.Combine(outputFolder, "Image");
            if (!Directory.Exists(sourceDir))
            {
                return;
            }

            var targetDir = Path.Combine(_appFileSystem.AppDataDirectory, "Image");
            _appFileSystem.MoveFolder(sourceDir, targetDir, SearchOption.TopDirectoryOnly);
        }

        private async Task RestoreAvatar(string outputFolder, string previousAvatarUri)
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

            await _appFileSystem.FileMoveAsync(sourceFilePath, targetFilePath);
            if (avatarUri != previousAvatarUri)
            {
                string previousAvatarPath = _mediaResourceManager.UrlRelativePathToFilePath(previousAvatarUri);
                if (File.Exists(previousAvatarPath))
                {
                    File.Delete(previousAvatarPath);
                }
            }
        }

        public void UseNewResourceUri(List<DiaryModel> diaries)
        {
            foreach (var diary in diaries)
            {
                if (!string.IsNullOrEmpty(diary.Content))
                {
                    diary.Content = diary.Content.Replace("appdata:///", "appdata/");
                    diary.Resources = _mediaResourceManager.GetDiaryResources(diary.Content);
                }

                diary.UpdateTime = DateTime.Now;
            }
        }

        public async Task AllUseNewResourceUriAsync()
        {
            var diaries = await _diaryService.QueryDiariesAsync();
            await _resourceService.DeleteAsync(it => it.ResourceUri.StartsWith("appdata:///"));
            UseNewResourceUri(diaries);
            await _diaryService.UpdateIncludesAsync(diaries);
        }

        private string GetCurrentDatabasePath()
        {
            bool privacyMode = _settingService.GetTemp(it => it.PrivacyMode);
            return privacyMode ? PrivacyDatabasePath : DatabasePath;
        }

        public async Task UpdateTemplateForOldDiaryAsync()
        {
#pragma warning disable CS0472
            await _diaryService.UpdateAsync(it => new() { Template = false }, it => it.Template == null);
#pragma warning restore CS0472
        }
    }
}
