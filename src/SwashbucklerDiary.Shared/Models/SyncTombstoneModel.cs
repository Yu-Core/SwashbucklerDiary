namespace SwashbucklerDiary.Shared
{
    public class SyncTombstoneModel : BaseModel
    {
        public string EntityName { get; set; } = string.Empty;

        public string EntityId { get; set; } = string.Empty;

        public string DeviceId { get; set; } = string.Empty;

        public DateTime DeletedAt { get; set; } = DateTime.Now;
    }
}
