namespace SwashbucklerDiary.Rcl.Essentials
{
    public abstract class AppFileSystem : IAppFileSystem
    {
        public const string AppDataVirtualDirectoryName = "appdata";

        public const string CacheVirtualDirectoryName = "cache";

        public abstract string AppDataDirectory { get; }

        public abstract string CacheDirectory { get; }

        public virtual Task SyncFS()
        {
            return Task.CompletedTask;
        }

        public async Task<string> CreateTempFileAsync(string fileName, string contents)
        {
            string path = CreateTempFilePath(fileName);
            await File.WriteAllTextAsync(path, contents).ConfigureAwait(false);
            return path;
        }

        public async Task<string> CreateTempFileAsync(string fileName, byte[] contents)
        {
            string path = CreateTempFilePath(fileName);
            await File.WriteAllBytesAsync(path, contents).ConfigureAwait(false);
            return path;
        }

        public async Task<string> CreateTempFileAsync(string fileName, Stream stream)
        {
            string path = CreateTempFilePath(fileName);

            await FileCopyAsync(stream, path);

            return path;
        }

        public void FileCopy(string sourceFilePath, string targetFilePath)
        {
            CreateFileDirectory(targetFilePath);
            File.Copy(sourceFilePath, targetFilePath, true);
        }

        public async Task FileCopyAsync(Stream stream, string path)
        {
            // 复位流
            if (stream.CanSeek)
                stream.Position = 0;

            // 自动 bufferSize
            int bufferSize = 81920; // 默认 80KB
            try
            {
                if (stream.CanSeek)
                {
                    long length = stream.Length;
                    bufferSize = length switch
                    {
                        < 1 * 1024 * 1024 => 81920,         // < 1MB → 80KB
                        < 100 * 1024 * 1024 => 1024 * 1024, // 1MB~100MB → 1MB
                        _ => 4 * 1024 * 1024                // ≥100MB → 4MB
                    };
                }
            }
            catch
            {
                // 有些流不支持 Length，保持默认
            }

            // 写文件
            await using var fileStream = new FileStream(
                path,
                FileMode.Create,
                FileAccess.Write,
                FileShare.None,
                bufferSize,
                FileOptions.Asynchronous | FileOptions.SequentialScan);

            await stream.CopyToAsync(fileStream, bufferSize).ConfigureAwait(false);
        }

        public async Task ClearFolderAsync(string targetDirectory, List<string>? excludeFiles = null)
        {
            if (!Directory.Exists(targetDirectory)) return;

            await Task.Run(() =>
            {
                var excludePaths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

                if (excludeFiles is not null && excludeFiles.Count != 0)
                {
                    foreach (var file in excludeFiles)
                    {
                        try
                        {
                            string absolutePath = Path.GetFullPath(file);
                            excludePaths.Add(absolutePath);
                        }
                        catch
                        {
                            // 忽略无效的路径
                        }
                    }
                }
                ClearDirectoryInternal(targetDirectory, excludePaths);
            }).ConfigureAwait(false);
        }

        private static void ClearDirectoryInternal(string directory, HashSet<string> excludePaths)
        {
            // 处理文件
            foreach (var file in Directory.GetFiles(directory))
            {
                string absolutePath = Path.GetFullPath(file);
                if (excludePaths.Contains(absolutePath))
                    continue;

                try { File.Delete(absolutePath); }
                catch { }
            }

            // 递归处理子目录
            foreach (var subDir in Directory.GetDirectories(directory))
            {
                string absolutePath = Path.GetFullPath(subDir);

                // 检查是否在排除列表中
                if (excludePaths.Contains(absolutePath))
                    continue;

                ClearDirectoryInternal(absolutePath, excludePaths);
                try { Directory.Delete(subDir); }
                catch { }
            }
        }

        public async Task<long> GetFolderSize(string path)
        {
            if (!Directory.Exists(path))
            {
                return 0;
            }

            var rootDir = new DirectoryInfo(path);
            long totalSize = 0;

            // 并行处理文件大小计算
            await Task.Run(() =>
            {
                // 获取所有文件（包括子目录）
                var allFiles = rootDir.GetFiles("*.*", SearchOption.AllDirectories);

                Parallel.ForEach(allFiles, file =>
                {
                    Interlocked.Add(ref totalSize, file.Length);
                });
            }).ConfigureAwait(false);

            return totalSize;
        }

        private static void CreateFileDirectory(string filePath)
        {
            string? directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath!);
            }
        }

        public void FileMove(string sourceFilePath, string targetFilePath)
        {
            CreateFileDirectory(targetFilePath);
            File.Move(sourceFilePath, targetFilePath);
        }

        public void MoveFolder(string sourceFolder, string destinationFolder, SearchOption searchOption, bool fileOverwrite = false)
        {
            var files = Directory.EnumerateFiles(sourceFolder, "*", searchOption);

            Parallel.ForEach(files, file =>
            {
                string relativePath = Path.GetRelativePath(sourceFolder, file);

                string destinationPath = Path.Combine(destinationFolder, relativePath);

                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);

                if (File.Exists(destinationPath))
                {
                    if (fileOverwrite)
                    {
                        File.Delete(destinationPath);
                    }
                    else
                    {
                        return;
                    }
                }

                File.Move(file, destinationPath);
            });
        }

        public async Task MoveFolderAsync(string sourceFolder, string destinationFolder, SearchOption searchOption, bool fileOverwrite = false)
        {
            await Task.Run(() =>
            {
                MoveFolder(sourceFolder, destinationFolder, searchOption, fileOverwrite);
            }).ConfigureAwait(false);
        }

        public Task ClearCacheAsync()
            => ClearFolderAsync(CacheDirectory);

        public async Task<string> GetCacheSizeAsync()
        {
            long fileSizeInBytes = await GetFolderSize(CacheDirectory);
            return ConvertBytesToReadable(fileSizeInBytes);
        }

        protected static string ConvertBytesToReadable(long bytes)
        {
            string[] sizes = ["B", "KB", "MB", "GB", "TB"];
            int i = 0;
            double size = bytes;

            while (size >= 1024 && i < sizes.Length - 1)
            {
                size /= 1024;
                i++;
            }

            return $"{size.ToString("0.#")} {sizes[i]}";
        }

        protected string CreateTempFilePath(string fileName)
        {
            // 在缓存目录下创建一个唯一的子目录
            string uniqueDir = Path.Combine(CacheDirectory, Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(uniqueDir);

            // 拼接最终路径
            return Path.Combine(uniqueDir, fileName);
        }
    }
}
