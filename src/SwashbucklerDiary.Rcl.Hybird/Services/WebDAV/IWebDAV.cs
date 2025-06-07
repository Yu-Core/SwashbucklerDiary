namespace SwashbucklerDiary.Rcl.Services
{
    public interface IWebDAV
    {
        bool Initialized { get; protected set; }

        Task Set(string? baseAddress, string? userName, string? password);

        Task<Stream> DownloadAsync(string destFileName);

        Task UploadAsync(string destFileName, Stream stream);

        Task<List<string>> GetZipFileListAsync(string folderName);
    }
}
