using ClosedXML.Excel;
using SwashbucklerDiary.Config;
using SwashbucklerDiary.Extensions;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
using System.Data;
using System.IO.Compression;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace SwashbucklerDiary.Services
{
    public class AppDataService : IAppDataService
    {
        private readonly IPlatformService PlatformService;
        private readonly II18nService I18n;
        private const string exportFileName = "SwashbucklerDiaryExport";
        private const string customScheme = "appdata:///";
        private const string ImageFolderName = "Image";
        private const string AudioFolderName = "Audio";
        private const string VideoFolderName = "Video";
        private static readonly string[] ResourceFolders = { ImageFolderName, AudioFolderName, VideoFolderName };

        public AppDataService(IPlatformService platformService,
            II18nService i18nService)
        {
            PlatformService = platformService;
            I18n = i18nService;
        }

        public void ClearCache() => ClearFolder(FileSystem.Current.CacheDirectory);

        public string GetCacheSize() => GetFolderSize(FileSystem.Current.CacheDirectory);

        public Task<bool> RestoreDatabase(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return Task.FromResult(false);
            }

            string outputFolder = Path.Combine(FileSystem.CacheDirectory, "DB");
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            else
            {
                ClearFolder(outputFolder);
            }

            ZipFile.ExtractToDirectory(filePath, outputFolder);
            // 获取文件夹下的db文件
            string[] jsonFiles = Directory.GetFiles(outputFolder, "*.db3");
            if (!jsonFiles.Any())
            {
                return Task.FromResult(false);
            }

            File.Copy(jsonFiles[0], SQLiteConstants.DatabasePath, true);
            ClearDiaryResource();
            RestoreDiaryResource(outputFolder);
            return Task.FromResult(true);
        }

        public async Task<bool> RestoreDatabase(Stream stream)
        {
            var path = Path.Combine(FileSystem.CacheDirectory, Guid.NewGuid().ToString() + ".zip");
            using (var fs = File.Create(path))
            {
                stream.CopyTo(fs);
            }
            var flag = await RestoreDatabase(path);
            File.Delete(path);
            return flag;
        }

        private static void ClearFolder(string folderPath)
        {
            if (!Directory.Exists(folderPath))
            {
                return;
            }

            DirectoryInfo directory = new(folderPath);

            // 删除文件夹中的所有文件
            foreach (FileInfo file in directory.GetFiles())
            {
                try
                {
                    file.Delete();
                }
                catch (Exception)
                {
                }
            }

            // 删除文件夹中的所有子文件夹
            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
            {
                ClearFolder(subDirectory.FullName);
                try
                {
                    subDirectory.Delete();
                }
                catch (Exception)
                {
                }
            }
        }

        private static string GetFolderSize(string path)
        {
            long fileSizeInBytes = GetDirectoryLength(path);
            return ConvertBytesToReadable(fileSizeInBytes);
        }
        private static long GetDirectoryLength(string dirPath)
        {
            long len = 0;
            //判断该路径是否存在（是否为文件夹）
            if (!Directory.Exists(dirPath))
            {
                //查询文件的大小
                len = FileSize(dirPath);
            }
            else
            {
                //定义一个DirectoryInfo对象
                DirectoryInfo di = new DirectoryInfo(dirPath);

                //通过GetFiles方法，获取di目录中的所有文件的大小
                foreach (FileInfo fi in di.GetFiles())
                {
                    len += fi.Length;
                }
                //获取di中所有的文件夹，并存到一个新的对象数组中，以进行递归
                DirectoryInfo[] dis = di.GetDirectories();
                if (dis.Length > 0)
                {
                    for (int i = 0; i < dis.Length; i++)
                    {
                        len += GetDirectoryLength(dis[i].FullName);
                    }
                }
            }
            return len;
        }

        private static long FileSize(string filePath)
        {
            //定义一个FileInfo对象，是指与filePath所指向的文件相关联，以获取其大小
            FileInfo fileInfo = new(filePath);
            return fileInfo.Length;
        }

        private static string ConvertBytesToReadable(long bytes)
        {
            string[] sizes = { "B", "KB", "MB", "GB", "TB" };
            int i = 0;
            double size = bytes;

            while (size >= 1024 && i < sizes.Length - 1)
            {
                size /= 1024;
                i++;
            }

            return $"{size.ToString("0.#")} {sizes[i]}";
        }

        private string CreateTxtContent(DiaryModel diary)
        {
            StringBuilder text = new();
            text.AppendLine(diary.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            text.AppendLine();

            if (!string.IsNullOrEmpty(diary.Weather))
            {
                text.AppendLine(I18n.T("Weather." + diary.Weather));
                text.AppendLine();
            }

            if (!string.IsNullOrEmpty(diary.Mood))
            {
                text.AppendLine(I18n.T("Mood." + diary.Mood));
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

        public Task<string> ExportTxtZipFileAsync(List<DiaryModel> diaries)
        {
            string outputFolder = Path.Combine(FileSystem.CacheDirectory, "Txt");
            string zipFilePath = Path.Combine(FileSystem.CacheDirectory, $"{exportFileName}Txt.zip");

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            else
            {
                ClearFolder(outputFolder);
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

        public Task<string> ExportJsonZipFileAsync(List<DiaryModel> diaries)
        {
            string outputFolder = Path.Combine(FileSystem.CacheDirectory, "Json");
            string zipFilePath = Path.Combine(FileSystem.CacheDirectory, $"{exportFileName}Json.zip");

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            else
            {
                ClearFolder(outputFolder);
            }

            foreach (var item in diaries)
            {
                string fileName = item.CreateTime.ToString("yyyy-MM-dd") + ".json";
                string filePath = Path.Combine(outputFolder, fileName);

                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                };

                string content = JsonSerializer.Serialize(item, options);
                WriteToFile(filePath, content);
            }

            CopyDiaryResource(diaries, outputFolder);

            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }

            // 将所有json文件添加到压缩包中
            ZipFile.CreateFromDirectory(outputFolder, zipFilePath);
            return Task.FromResult(zipFilePath);
        }

        public Task<string> ExportMdZipFileAsync(List<DiaryModel> diaries)
        {
            string outputFolder = Path.Combine(FileSystem.CacheDirectory, "Markdown");
            string zipFilePath = Path.Combine(FileSystem.CacheDirectory, $"{exportFileName}Markdown.zip");

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            else
            {
                ClearFolder(outputFolder);
            }

            foreach (var item in diaries)
            {
                string fileName = item.CreateTime.ToString("yyyy-MM-dd") + ".md";
                string filePath = Path.Combine(outputFolder, fileName);

                var content = item.Content?.Replace(customScheme, "./");
                WriteToFile(filePath, content);
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
        public Task<string> ExportXlsxFileAsync(List<DiaryModel> diaries)
        {
            string filePath = Path.Combine(FileSystem.CacheDirectory, $"{exportFileName}Xlsx.xlsx");
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            var dataTable = new DataTable();
            dataTable.Columns.Add(I18n.T("Excel.Time"));
            dataTable.Columns.Add(I18n.T("Excel.Weather"));
            dataTable.Columns.Add(I18n.T("Excel.Mood"));
            dataTable.Columns.Add(I18n.T("Excel.Location"));
            dataTable.Columns.Add(I18n.T("Excel.Tags"));
            dataTable.Columns.Add(I18n.T("Excel.Title"));
            dataTable.Columns.Add(I18n.T("Excel.Content"));

            foreach (var item in diaries)
            {
                var time = item.CreateTime.ToString("yyyy/MM/dd HH:mm:ss");
                var weather = item.Weather is null ? string.Empty : I18n.T("Weather." + item.Weather);
                var mood = item.Mood is null ? string.Empty : I18n.T("Mood." + item.Mood);
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

        private async Task<bool> CreateFileAndSaveAsync(Func<List<DiaryModel>, Task<string>> func, string type, List<DiaryModel> diaries)
        {
            string filePath = await func.Invoke(diaries);
            string extension = Path.GetExtension(filePath);
            string saveFilePath = $"{exportFileName}{type}{DateTime.Now:yyyy-MM-dd-HH-mm-ss}{extension}";
            var path = await PlatformService.SaveFileAsync(saveFilePath, filePath);
            return path is not null;
        }

        public Task<bool> ExportTxtZipFileAndSaveAsync(List<DiaryModel> diaries)
            => CreateFileAndSaveAsync(ExportTxtZipFileAsync, "Txt", diaries);

        public Task<bool> ExportJsonZipFileAndSaveAsync(List<DiaryModel> diaries)
            => CreateFileAndSaveAsync(ExportJsonZipFileAsync, "Json", diaries);

        public Task<bool> ExportMdZipFileAndSaveAsync(List<DiaryModel> diaries)
            => CreateFileAndSaveAsync(ExportMdZipFileAsync, "Markdown", diaries);

        public Task<bool> ExportXlsxFileAndSaveAsync(List<DiaryModel> diaries)
            => CreateFileAndSaveAsync(ExportXlsxFileAsync, "Xlsx", diaries);

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

        public async Task<string> CreateCacheFileAsync(string filePath, string contents)
        {
            CreateDirectory(filePath, FileSystem.Current.CacheDirectory);
            string path = Path.Combine(FileSystem.Current.CacheDirectory, filePath);
            await File.WriteAllTextAsync(path, contents);
            return path;
        }

        public async Task<string> CreateCacheFileAsync(string filePath, byte[] contents)
        {
            CreateDirectory(filePath, FileSystem.Current.CacheDirectory);
            string path = Path.Combine(FileSystem.Current.CacheDirectory, filePath);
            await File.WriteAllBytesAsync(path, contents);
            return path;
        }

        private static void CreateDirectory(string filePath, string prefix)
        {
            string? directoryPath = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directoryPath))
            {
                directoryPath = Path.Combine(prefix, directoryPath);
                if (!Directory.Exists(directoryPath))
                {
                    Directory.CreateDirectory(directoryPath);
                }
            }
        }

        public async Task<string> CreateAppDataFileAsync(string targetFilePath, string sourceFilePath)
        {
            bool isMD5 = false;
            if (string.IsNullOrEmpty(Path.GetExtension(targetFilePath)))
            {
                isMD5 = true;
                var fn = sourceFilePath.FileMD5() + Path.GetExtension(sourceFilePath);
                targetFilePath = targetFilePath.TrimEnd('/') + "/" + fn;
            }

            string relativePath = Path.Combine(targetFilePath.Split('/'));
            CreateDirectory(relativePath, FileSystem.Current.AppDataDirectory);
            string path = Path.Combine(FileSystem.Current.AppDataDirectory, relativePath);
            if (File.Exists(path))
            {
                if (isMD5)
                {
                    return customScheme + targetFilePath;
                }

                File.Delete(path);
            }

            var file = new FileResult(sourceFilePath);
#if WINDOWS
            // on Windows file.OpenReadAsync() throws an exception
            using Stream sourceStream = File.OpenRead(sourceFilePath);
#else
            using Stream sourceStream = await file.OpenReadAsync();
#endif
            using (FileStream localFileStream = File.OpenWrite(path))
            {
                await sourceStream.CopyToAsync(localFileStream);
            };

            return customScheme + targetFilePath;
        }

        public Task<string> CreateAppDataImageFileAsync(string filePath)
            => CreateAppDataFileAsync(ImageFolderName, filePath);

        public Task<string> CreateAppDataAudioFileAsync(string filePath)
            => CreateAppDataFileAsync(AudioFolderName, filePath);

        public Task<string> CreateAppDataVideoFileAsync(string filePath)
            => CreateAppDataFileAsync(VideoFolderName, filePath);

        public Task<bool> DeleteAppDataFileByFilePathAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return Task.FromResult(false);
            }

            File.Delete(filePath);
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAppDataFileByCustomSchemeAsync(string uri)
        {
            string path = CustomSchemeUriToFilePath(uri);
            return DeleteAppDataFileByFilePathAsync(path);
        }

        public string CustomSchemeUriToFilePath(string uri)
        {
            var relativePath = Path.Combine(uri.TrimStart(customScheme.ToCharArray()).Split('/'));
            return Path.Combine(FileSystem.Current.AppDataDirectory, relativePath);
        }

        private static void CopyUriFileToOutFolder(string uri, string outFolder)
        {
            uri = uri.Replace(customScheme, "");
            var relativePath = Path.Combine(uri.TrimStart(customScheme.ToCharArray()).Split('/'));
            var filePath = Path.Combine(FileSystem.Current.AppDataDirectory, relativePath);
            if (!File.Exists(filePath))
            {
                return;
            }

            var outFilePath = Path.Combine(outFolder, relativePath);
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

        public async Task<List<DiaryModel>> ImportJsonFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return new();
            }

            string outputFolder = Path.Combine(FileSystem.CacheDirectory, "Json");
            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            else
            {
                ClearFolder(outputFolder);
            }

            ZipFile.ExtractToDirectory(filePath, outputFolder);
            // 获取文件夹下的所有json文件
            string[] jsonFiles = Directory.GetFiles(outputFolder, "*.json");
            if (jsonFiles.Length == 0)
            {
                return new();
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
            return diaries;
        }

        static void CopyFolder(string sourceFolder, string destinationFolder, SearchOption searchOption)
        {
            // 获取源文件夹中的所有文件
            string[] files = Directory.GetFiles(sourceFolder, "*", searchOption);

            foreach (string file in files)
            {
                // 获取文件在源文件夹中的相对路径
                string relativePath = Path.GetRelativePath(sourceFolder, file);

                // 构建目标文件的路径
                string destinationPath = Path.Combine(destinationFolder, relativePath);

                // 确保目标文件夹存在
                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);

                // 复制文件
                File.Copy(file, destinationPath, true);
            }
        }

        public async Task<Stream> BackupDatabase(List<DiaryModel> diaries, bool copyResources)
        {
            string filePath = await ExportDBZipFileAsync(diaries, copyResources);
            return File.OpenRead(filePath);
        }

        public async Task<string?> BackupDatabase(string path, List<DiaryModel> diaries, bool copyResources)
        {
            string filePath = await ExportDBZipFileAsync(diaries, copyResources);
            string destFileName = GetBackupFileName();
            return await PlatformService.SaveFileAsync(path, destFileName, filePath);
        }

        public Task<string> ExportDBZipFileAsync(List<DiaryModel> diaries, bool copyResources)
        {
            string outputFolder = Path.Combine(FileSystem.CacheDirectory, "DB");
            string zipFilePath = Path.Combine(FileSystem.CacheDirectory, $"{exportFileName}DB.zip");

            if (!Directory.Exists(outputFolder))
            {
                Directory.CreateDirectory(outputFolder);
            }
            else
            {
                ClearFolder(outputFolder);
            }

            var destFileName = Path.Combine(outputFolder, SQLiteConstants.DatabaseFilename);
            File.Copy(SQLiteConstants.DatabasePath, destFileName);
            if (copyResources)
            {
                CopyDiaryResource(diaries, outputFolder);
            }

            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }

            ZipFile.CreateFromDirectory(outputFolder, zipFilePath);
            return Task.FromResult(zipFilePath);
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

        private void RestoreDiaryResource(string outputFolder)
        {
            string[] subfolders = Directory.GetDirectories(outputFolder);
            foreach (string subfolder in subfolders)
            {
                var name = Path.GetFileName(subfolder);
                if (ResourceFolders.Contains(name))
                {
                    var outpath = Path.Combine(FileSystem.AppDataDirectory, name);
                    CopyFolder(subfolder, outpath, SearchOption.TopDirectoryOnly);
                }
            }
        }

        public string GetBackupFileName()
        {
            string name = "SwashbucklerDiaryBackups";
            string time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
            string version = $"v{PlatformService.GetAppVersion()}";
            string extension = ".zip";
            return $"{name}{time}{version}{extension}";
        }

        private void ClearDiaryResource()
        {
            foreach (var item in ResourceFolders)
            {
                var folderPath = Path.Combine(FileSystem.AppDataDirectory, item);
                ClearFolder(folderPath);
            }
        }
    }
}
