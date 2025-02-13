namespace SwashbucklerDiary.Rcl.Essentials
{
    public abstract class AppFileSystem : IAppFileSystem
    {
        public static readonly string AppDataVirtualDirectoryName = "appdata";

        public static readonly string CacheVirtualDirectoryName = "cache";

        public abstract string AppDataDirectory { get; }

        public abstract string CacheDirectory { get; }

        public async Task<string> CreateTempFileAsync(string fileName, string contents)
        {
            string path = Path.Combine(CacheDirectory, fileName);
            await File.WriteAllTextAsync(path, contents);
            return path;
        }

        public async Task<string> CreateTempFileAsync(string fileName, byte[] contents)
        {
            string path = Path.Combine(CacheDirectory, fileName);
            await File.WriteAllBytesAsync(path, contents);
            return path;
        }

        public async Task<string> CreateTempFileAsync(string fileName, Stream stream)
        {
            string path = Path.Combine(CacheDirectory, fileName);
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }
            return path;
        }

        public async Task FileCopyAsync(string targetFilePath, string sourceFilePath)
        {
            using Stream sourceStream = File.OpenRead(sourceFilePath);
            await FileCopyAsync(targetFilePath, sourceStream);
        }

        public async Task FileCopyAsync(string targetFilePath, Stream sourceStream)
        {
            CreateFileDirectory(targetFilePath);

            using (FileStream localFileStream = File.OpenWrite(targetFilePath))
            {
                await sourceStream.CopyToAsync(localFileStream, 1024 * 1024);
            };
        }

        public void ClearFolder(string folderPath, List<string>? exceptPaths = null)
        {
            if (!Directory.Exists(folderPath))
            {
                return;
            }

            DirectoryInfo directory = new(folderPath);

            var files = directory.GetFiles();
            if (exceptPaths != null && exceptPaths.Count != 0)
            {
                files = directory.GetFiles().Where(it => !exceptPaths.Contains(it.FullName)).ToArray();
            }

            // 删除文件夹中的所有文件
            foreach (FileInfo file in files)
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

        public long GetFolderSize(string path)
        {
            long len = 0;
            //判断该路径是否存在（是否为文件夹）
            if (!Directory.Exists(path))
            {
                //查询文件的大小
                if (File.Exists(path))
                {
                    len = FileSize(path);
                }
            }
            else
            {
                //定义一个DirectoryInfo对象
                DirectoryInfo dir = new DirectoryInfo(path);

                //通过GetFiles方法，获取di目录中的所有文件的大小
                foreach (FileInfo fi in dir.GetFiles())
                {
                    len += fi.Length;
                }
                //获取di中所有的文件夹，并存到一个新的对象数组中，以进行递归
                DirectoryInfo[] dirs = dir.GetDirectories();
                if (dirs.Length > 0)
                {
                    for (int i = 0; i < dirs.Length; i++)
                    {
                        len += GetFolderSize(dirs[i].FullName);
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

        private static void CreateFileDirectory(string filePath)
        {
            string? directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath!);
            }
        }

        public Task FileMoveAsync(string sourceFilePath, string targetFilePath)
        {
            CreateFileDirectory(targetFilePath);
            File.Move(sourceFilePath, targetFilePath);
            return Task.CompletedTask;
        }

        public void CopyFolder(string sourceFolder, string destinationFolder, SearchOption searchOption)
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
                if (!File.Exists(destinationPath))
                {
                    File.Copy(file, destinationPath);
                }
            }
        }

        public void MoveFolder(string sourceFolder, string destinationFolder, SearchOption searchOption)
        {
            string[] files = Directory.GetFiles(sourceFolder, "*", searchOption);

            foreach (string file in files)
            {
                string relativePath = Path.GetRelativePath(sourceFolder, file);

                string destinationPath = Path.Combine(destinationFolder, relativePath);

                Directory.CreateDirectory(Path.GetDirectoryName(destinationPath)!);

                if (!File.Exists(destinationPath))
                {
                    File.Move(file, destinationPath);
                }
            }
        }

        public void ClearCache()
            => ClearFolder(CacheDirectory);

        public string GetCacheSize()
        {
            long fileSizeInBytes = GetFolderSize(CacheDirectory);
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
    }
}
