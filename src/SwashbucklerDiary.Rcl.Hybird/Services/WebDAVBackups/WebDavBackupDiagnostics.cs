namespace SwashbucklerDiary.Rcl.Services
{
    public class WebDavBackupDiagnostics
    {
        public bool Success { get; set; }

        public int IncrementalBackupCount { get; set; }

        public int LegacyBackupCount { get; set; }

        public DateTime? LatestBackupTime { get; set; }

        public long TotalSize { get; set; }

        public string? ErrorMessage { get; set; }
    }
}
