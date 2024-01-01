namespace SwashbucklerDiary.WebAssembly.Essentials
{
    public class AppFileManager : Rcl.Essentials.AppFileManager
    {
        public override async Task<string> CreateTempFileAsync(string fileName, string contents)
        {
            string path = Path.Combine(FileSystem.CacheDirectory, fileName);
            await File.WriteAllTextAsync(path, contents);
            return path;
        }

        public override async Task<string> CreateTempFileAsync(string fileName, byte[] contents)
        {
            string path = Path.Combine(FileSystem.CacheDirectory, fileName);
            await File.WriteAllBytesAsync(path, contents);
            return path;
        }

        public override async Task<string> CreateTempFileAsync(string fileName, Stream stream)
        {
            string path = Path.Combine(FileSystem.CacheDirectory, fileName);
            using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                await stream.CopyToAsync(fileStream);
            }
            return path;
        }
    }
}
