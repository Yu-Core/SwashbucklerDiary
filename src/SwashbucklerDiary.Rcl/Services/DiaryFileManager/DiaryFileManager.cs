using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using SwashbucklerDiary.Rcl.Essentials;
using SwashbucklerDiary.Shared;
using System.Data;
using System.IO.Compression;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace SwashbucklerDiary.Rcl.Services
{
    public abstract class DiaryFileManager : IDiaryFileManager
    {
        protected readonly IAppFileManager _appFileManager;

        protected readonly IPlatformIntegration _platformIntegration;

        protected readonly II18nService _i18n;

        protected readonly IMediaResourceManager _mediaResourceManager;

        protected readonly IDiaryService _diaryService;

        protected const string exportFileNamePrefix = "SDExport";

        protected const string backupFileNamePrefix = "SDBackup";

        protected JsonSerializerOptions jsonSerializerOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        };

        public DiaryFileManager(IAppFileManager appFileManager,
            IPlatformIntegration platformIntegration,
            II18nService i18nService,
            IMediaResourceManager mediaResourceManager,
            IDiaryService diaryService)
        {
            _appFileManager = appFileManager;
            _platformIntegration = platformIntegration;
            _i18n = i18nService;
            _mediaResourceManager = mediaResourceManager;
            _diaryService = diaryService;
        }

        public abstract Task<string> ExportDBAsync(bool copyResources);

        public abstract Task<string> ExportJsonAsync(List<DiaryModel> diaries);

        public abstract Task<string> ExportMdAsync(List<DiaryModel> diaries);

        public abstract Task<string> ExportTxtAsync(List<DiaryModel> diaries);

        public abstract Task<string> ExportXlsxAsync(List<DiaryModel> diaries);

        protected Task<string> InternalExportDBAsync(bool copyResources, string outputFolder, string zipFilePath)
        {
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            else
            {
                _appFileManager.ClearFolder(outputFolder);
            }

            string databasePath = GetDatabasePath();
            var destFileName = Path.Combine(outputFolder, Path.GetFileName(databasePath));
            File.Copy(databasePath, destFileName);
            if (copyResources)
            {
                CopyDiaryResource(outputFolder);
            }

            CreateExportVersionInfo(outputFolder, ".db3");

            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }

            ZipFile.CreateFromDirectory(outputFolder, zipFilePath);
            return Task.FromResult(zipFilePath);
        }

        protected Task<string> InternalExportJsonAsync(List<DiaryModel> diaries, string outputFolder, string zipFilePath)
        {
            string fileSuffix = ".json";

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            else
            {
                _appFileManager.ClearFolder(outputFolder);
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

        protected Task<string> InternalExportMdAsync(List<DiaryModel> diaries, string outputFolder, string zipFilePath)
        {
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            else
            {
                _appFileManager.ClearFolder(outputFolder);
            }

            foreach (var item in diaries)
            {
                string fileName = item.CreateTime.ToString("yyyy-MM-dd") + ".md";
                string filePath = Path.Combine(outputFolder, fileName);

                WriteToFile(filePath, item.Content);
            }

            CopyDiaryResource(diaries, outputFolder);

            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }

            // 将所有.md文件添加到压缩包中
            ZipFile.CreateFromDirectory(outputFolder, zipFilePath);
            return Task.FromResult(zipFilePath);
        }

        protected Task<string> InternalExportTxtAsync(List<DiaryModel> diaries, string outputFolder, string zipFilePath)
        {
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            else
            {
                _appFileManager.ClearFolder(outputFolder);
            }

            foreach (var item in diaries)
            {
                string fileName = item.CreateTime.ToString("yyyy-MM-dd") + ".txt";
                string filePath = Path.Combine(outputFolder, fileName);
                string content = CreateTxtContent(item);
                WriteToFile(filePath, content);
            }

            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }

            ZipFile.CreateFromDirectory(outputFolder, zipFilePath);
            return Task.FromResult(zipFilePath);
        }

        protected Task<string> InternalExportXlsxAsync(List<DiaryModel> diaries, string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var dataTable = new DataTable();
            dataTable.Columns.Add(_i18n.T("Excel.Time"));
            dataTable.Columns.Add(_i18n.T("Excel.Weather"));
            dataTable.Columns.Add(_i18n.T("Excel.Mood"));
            dataTable.Columns.Add(_i18n.T("Excel.Location"));
            dataTable.Columns.Add(_i18n.T("Excel.Tags"));
            dataTable.Columns.Add(_i18n.T("Excel.Title"));
            dataTable.Columns.Add(_i18n.T("Excel.Content"));

            foreach (var item in diaries)
            {
                var time = item.CreateTime.ToString("yyyy/MM/dd HH:mm:ss");
                var weather = item.Weather is null ? string.Empty : _i18n.T("Weather." + item.Weather);
                var mood = item.Mood is null ? string.Empty : _i18n.T("Mood." + item.Mood);
                var location = item.Location ?? string.Empty;
                var tags = string.Empty;
                if (item.Tags is not null && item.Tags.Count > 0)
                {
                    foreach (var tag in item.Tags)
                    {
                        tags += tag.Name + ", ";
                    }
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

        protected abstract void CopyUriFileToOutFolder(string uri, string outFolder);

        protected abstract string GetDatabasePath();

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
                Version = _platformIntegration.AppVersion,
                FileSuffix = fileSuffix,
                Platform = _platformIntegration.CurrentPlatform.ToString(),
                DateTime = DateTime.Now,
            };

            // 将对象序列化为 JSON 字符串
            string jsonString = JsonSerializer.Serialize(exportVersionInfo);

            // 将 JSON 字符串写入文件
            var jsonPath = Path.Combine(outputFolder, "version.json");
            File.WriteAllText(jsonPath, jsonString);
        }

        private void CopyDiaryResource(List<DiaryModel> diaries, string outputFolder)
        {
            var resources = diaries.SelectMany(a => a.Resources ?? new()).Distinct().ToList();

            foreach (var resource in resources)
            {
                if (resource.ResourceUri is null)
                {
                    continue;
                }

                CopyUriFileToOutFolder(resource.ResourceUri, outputFolder);
            }
        }

        protected abstract void CopyDiaryResource(string outputFolder);

        private string CreateTxtContent(DiaryModel diary)
        {
            StringBuilder text = new();
            text.AppendLine(diary.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            text.AppendLine();

            if (!string.IsNullOrEmpty(diary.Weather))
            {
                text.AppendLine(_i18n.T("Weather." + diary.Weather));
                text.AppendLine();
            }

            if (!string.IsNullOrEmpty(diary.Mood))
            {
                text.AppendLine(_i18n.T("Mood." + diary.Mood));
            }

            if (!string.IsNullOrEmpty(diary.Location))
            {
                text.AppendLine(diary.Location);
                text.AppendLine();
            }

            if (diary.Tags is not null && diary.Tags.Count > 0)
            {
                foreach (var tag in diary.Tags)
                {
                    text.Append(tag.Name + ", ");
                }
                text.AppendLine();
                text.AppendLine();
            }

            if (!string.IsNullOrEmpty(diary.Title))
            {
                text.AppendLine(diary.Title);
                text.AppendLine();
            }

            text.AppendLine(diary.Content);
            text.AppendLine();
            return text.ToString();
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

        private string InternalGetExportFileName(string prefix,string suffix)
        {
            string dataTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            string version = _platformIntegration.AppVersion;
            return $"{prefix}_{dataTime}_v{version}{suffix}";
        }

        public abstract Task<bool> ImportDBAsync(string filePath);

        public async Task<bool> ImportDBAsync(Stream stream)
        {
            string fileName = Guid.NewGuid().ToString() + ".zip";
            string path = await _appFileManager.CreateTempFileAsync(fileName, stream);
            var flag = await ImportDBAsync(path);
            File.Delete(path);
            return flag;
        }

        protected Task<bool> InternalImportDBAsync(string filePath, string outputFolder)
        {
            if (!File.Exists(filePath))
            {
                return Task.FromResult(false);
            }


            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            else
            {
                _appFileManager.ClearFolder(outputFolder);
            }

            ZipFile.ExtractToDirectory(filePath, outputFolder);
            var versionJsonPath = Path.Combine(outputFolder, "version.json");
            if (!File.Exists(versionJsonPath))
            {
                return Task.FromResult(false);
            }
            else
            {
                File.Delete(versionJsonPath);
            }

            // 获取文件夹下的db文件
            string[] jsonFiles = Directory.GetFiles(outputFolder, "*.db3");
            if (jsonFiles.Length == 0)
            {
                return Task.FromResult(false);
            }

            File.Copy(jsonFiles[0], GetDatabasePath(), true);
            ClearAllDiaryResources();
            RestoreDiaryResource(outputFolder);
            return Task.FromResult(true);
        }

        public abstract Task<bool> ImportJsonAsync(string filePath);

        protected async Task<bool> InternalImportJsonAsync(string filePath, string outputFolder)
        {
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
                _appFileManager.ClearFolder(outputFolder);
            }

            ZipFile.ExtractToDirectory(filePath, outputFolder);
            var versionJsonPath = Path.Combine(outputFolder, "version.json");
            if (!File.Exists(versionJsonPath))
            {
                return false;
            }
            else
            {
                File.Delete(versionJsonPath);
            }

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
                var diarie = await JsonSerializer.DeserializeAsync<DiaryModel>(openStream);
                if (diarie is not null)
                {
                    diaries.Add(diarie);
                }
            }

            RestoreDiaryResource(outputFolder);
            return await _diaryService.ImportAsync(diaries);
        }

        protected abstract void ClearAllDiaryResources();

        protected abstract void RestoreDiaryResource(string outputFolder);
    }

    public class ExportVersionInfo
    {
        public string? Version { get; set; }

        public string? FileSuffix { get; set; }

        public string? Platform { get; set; }

        public DateTime DateTime { get; set; }
    }
}
