namespace SwashbucklerDiary.Shared
{
    public class ResourceModel
    {
        public string? ResourceUri { get; set; }

        public MediaResource ResourceType { get; set; }

        public List<DiaryModel>? Diaries { get; set; }
    }
}
