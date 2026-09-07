namespace SwashbucklerDiary.Rcl.Services
{
    public class WebDAVFileInfo
    {
        public required string Name { get; set; }

        public long? Length { get; set; }

        public DateTime? LastModified { get; set; }

        public bool IsCollection { get; set; }
    }
}
