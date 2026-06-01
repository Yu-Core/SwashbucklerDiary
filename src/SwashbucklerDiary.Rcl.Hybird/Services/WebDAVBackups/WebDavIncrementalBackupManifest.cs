namespace SwashbucklerDiary.Rcl.Services
{
    public class WebDavIncrementalBackupManifest
    {
        public int Version { get; set; } = 1;

        public string BackupId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public string AppVersion { get; set; } = string.Empty;

        public bool LegacyZip { get; set; }

        public string? LegacyRemotePath { get; set; }

        public long? LegacyLength { get; set; }

        public List<WebDavIncrementalBackupFile> Files { get; set; } = [];
    }

    public class WebDavIncrementalBackupFile
    {
        public string Kind { get; set; } = string.Empty;

        public string Hash { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string RemotePath { get; set; } = string.Empty;

        public string? ResourceUri { get; set; }

        public long Length { get; set; }
    }
}
