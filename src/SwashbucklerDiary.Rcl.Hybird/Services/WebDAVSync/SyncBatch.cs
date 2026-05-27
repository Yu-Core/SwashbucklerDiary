using SwashbucklerDiary.Shared;

namespace SwashbucklerDiary.Rcl.Services
{
    public class SyncBatch
    {
        public int Version { get; set; } = 2;

        public string BatchId { get; set; } = string.Empty;

        public string DeviceId { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<SyncDiaryChange> Diaries { get; set; } = [];
    }

    public class SyncDiaryChange
    {
        public string Operation { get; set; } = string.Empty;

        public Guid Id { get; set; }

        public DateTime UpdatedAt { get; set; }

        public DiaryModel? Diary { get; set; }

        public List<SyncTagSnapshot> Tags { get; set; } = [];

        public List<SyncResourceSnapshot> Resources { get; set; } = [];
    }

    public class SyncTagSnapshot
    {
        public Guid Id { get; set; }

        public DateTime CreateTime { get; set; }

        public DateTime UpdateTime { get; set; }

        public string? Name { get; set; }
    }

    public class SyncResourceSnapshot
    {
        public string ResourceUri { get; set; } = string.Empty;

        public MediaResource ResourceType { get; set; }
    }
}
