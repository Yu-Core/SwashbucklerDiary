namespace SwashbucklerDiary.Shared
{
    public class SyncConflictModel : BaseModel
    {
        public string EntityName { get; set; } = string.Empty;

        public string EntityId { get; set; } = string.Empty;

        public string BatchId { get; set; } = string.Empty;

        public string DeviceId { get; set; } = string.Empty;

        public string RemotePath { get; set; } = string.Empty;

        public DateTime LocalUpdateTime { get; set; }

        public DateTime RemoteUpdateTime { get; set; }

        public string Reason { get; set; } = string.Empty;
    }
}
