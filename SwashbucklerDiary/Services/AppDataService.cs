using SwashbucklerDiary.Config;
using SwashbucklerDiary.IServices;
using SwashbucklerDiary.Models;
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

        public AppDataService(IPlatformService platformService,
            II18nService i18nService)
        {
            PlatformService = platformService;
            I18n = i18nService;
        }

        public void ClearCache() => ClearFolder(FileSystem.Current.CacheDirectory);

        public string GetCacheSize() => GetFolderSize(FileSystem.Current.CacheDirectory);

        public string BackupFileName
        {
            get
            {
                string name = "SwashbucklerDiaryBackups";
                string time = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
                string version = $"v{PlatformService.GetAppVersion()}";
                string suffix = ".db3";

                return name + time + version + suffix;
            }
        }

        public FileStream GetDatabaseStream()
        {
            return File.OpenRead(SQLiteConstants.DatabasePath);
        }

        public void RestoreDatabase(string path)
        {
            File.Copy(path, SQLiteConstants.DatabasePath, true);
        }

        public void RestoreDatabase(Stream stream)
        {
            using var fileStream = new FileStream(SQLiteConstants.DatabasePath, FileMode.Create);
            stream.CopyTo(fileStream);
        }

        private void ClearFolder(string folderPath)
        {
            DirectoryInfo directory = new(folderPath);

            // 删除文件夹中的所有文件
            foreach (FileInfo file in directory.GetFiles())
            {
                file.Delete();
            }

            // 删除文件夹中的所有子文件夹
            foreach (DirectoryInfo subDirectory in directory.GetDirectories())
            {
                ClearFolder(subDirectory.FullName);
                subDirectory.Delete();
            }
        }

        private string GetFolderSize(string path)
        {
            long fileSizeInBytes = GetDirectoryLength(path);
            return ConvertBytesToReadable(fileSizeInBytes);
        }
        private long GetDirectoryLength(string dirPath)
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
            FileInfo fileInfo = new FileInfo(filePath);
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

        public async Task<string> CreateTxtFileAsync(List<DiaryModel> diaries)
        {
            StringBuilder text = new();
            foreach (var item in diaries)
            {
                text.AppendLine(item.CreateTime.ToString("yyyy-MM-dd HH:mm:ss"));
                if (string.IsNullOrEmpty(item.Title))
                {
                    text.AppendLine(item.Title);
                }

                if (!string.IsNullOrEmpty(item.Weather))
                {
                    text.AppendLine(I18n.T("Weather." + item.Weather));
                }

                if (!string.IsNullOrEmpty(item.Mood))
                {
                    text.AppendLine(I18n.T("Weather." + item.Weather));
                }

                if (!string.IsNullOrEmpty(item.Location))
                {
                    text.AppendLine(item.Location);
                }

                text.AppendLine(item.Content);
                if (item.Tags is not null && item.Tags.Count > 0)
                {
                    foreach (var tag in item.Tags)
                    {
                        text.Append(tag + " ");
                    }
                    text.AppendLine();
                }

                text.AppendLine();
            }

            string filePath = Path.Combine(FileSystem.CacheDirectory, $"{exportFileName}.txt");
            await File.WriteAllTextAsync(filePath, text.ToString());
            return filePath;
        }

        public async Task<string> CreateJsonFileAsync(List<DiaryModel> diaries)
        {
            string filePath = Path.Combine(FileSystem.CacheDirectory, $"{exportFileName}.json");

            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            };

            string jsonString = JsonSerializer.Serialize(diaries, options);
            await File.WriteAllTextAsync(filePath, jsonString);
            return filePath;
        }

        public Task<string> CreateMdFileAsync(List<DiaryModel> diaries)
        {
            string outputFolder = Path.Combine(FileSystem.CacheDirectory, "Markdown");
            string zipFilePath = Path.Combine(FileSystem.CacheDirectory, $"{exportFileName}.zip");

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
                string fileName = item.CreateTime.ToString("yyyy-MM-dd-HH-mm-ss") + ".md";
                string filePath = Path.Combine(outputFolder, fileName);
                WriteToFile(filePath, item.Content);
            }

            if (File.Exists(zipFilePath))
            {
                File.Delete(zipFilePath);
            }

            // 将所有.md文件添加到压缩包中
            ZipFile.CreateFromDirectory(outputFolder, zipFilePath);
            return Task.FromResult(zipFilePath);
        }

        public async Task<bool> CreateTxtFileAndSaveAsync(List<DiaryModel> diaries)
        {
            string filePath = await CreateTxtFileAsync(diaries);
            string saveFilePath = exportFileName + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt";
            return await SaveFile(saveFilePath, filePath);
        }

        public async Task<bool> CreateJsonFileAndSaveAsync(List<DiaryModel> diaries)
        {
            string filePath = await CreateJsonFileAsync(diaries);
            string saveFilePath = exportFileName + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".json";
            return await SaveFile(saveFilePath, filePath);
        }

        public async Task<bool> CreateMdFileAndSaveAsync(List<DiaryModel> diaries)
        {
            string filePath = await CreateMdFileAsync(diaries);
            string saveFilePath = exportFileName + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".zip";
            return await SaveFile(saveFilePath, filePath);
        }

        private async Task<bool> SaveFile(string targetFilePath, string sourceFilePath)
        {
            using FileStream stream = File.OpenRead(sourceFilePath);
            //Cannot save an existing file
            //https://github.com/CommunityToolkit/Maui/issues/1049
            var filePath = await PlatformService.SaveFileAsync(targetFilePath, stream);
            if (filePath == null)
            {
                return false;
            }

            return true;
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

        public async Task<string> CreateAppDataFileAsync(string filePath, string sourcePath)
        {
            CreateDirectory(filePath, FileSystem.Current.AppDataDirectory);
            string path = Path.Combine(FileSystem.Current.AppDataDirectory, filePath);
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            var file = new FileResult(sourcePath);
#if WINDOWS
            // on Windows file.OpenReadAsync() throws an exception
            using Stream sourceStream = File.OpenRead(sourcePath);
#else
            using Stream sourceStream = await file.OpenReadAsync();
#endif
            using (FileStream localFileStream = File.OpenWrite(path))
            {
                await sourceStream.CopyToAsync(localFileStream);
            };
            return path;
        }
    }
}
