namespace SwashbucklerDiary.Shared
{
    public class SyncLogModel : BaseModel
    {
        public string Operation { get; set; } = string.Empty;

        public bool Success { get; set; }

        public int Pushed { get; set; }

        public int Pulled { get; set; }

        public int Deleted { get; set; }

        public int Conflicts { get; set; }

        public int FileCount { get; set; }

        public int IncrementalBackupCount { get; set; }

        public int LegacyBackupCount { get; set; }

        public long TotalSize { get; set; }

        public string? Message { get; set; }
    }
}
