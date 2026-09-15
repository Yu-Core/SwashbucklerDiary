namespace SwashbucklerDiary.Shared
{
    public class SyncStateModel : BaseModel
    {
        public string BatchId { get; set; } = string.Empty;

        public string DeviceId { get; set; } = string.Empty;

        public string RemotePath { get; set; } = string.Empty;

        public DateTime AppliedAt { get; set; } = DateTime.UtcNow;
    }
}
