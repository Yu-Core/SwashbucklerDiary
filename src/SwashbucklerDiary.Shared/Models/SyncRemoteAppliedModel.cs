namespace SwashbucklerDiary.Shared
{
    public class SyncRemoteAppliedModel : BaseModel
    {
        public string EntityName { get; set; } = string.Empty;

        public string EntityId { get; set; } = string.Empty;

        public DateTime RemoteUpdateTime { get; set; }

        public string BatchId { get; set; } = string.Empty;

        public string RemotePath { get; set; } = string.Empty;
    }
}
